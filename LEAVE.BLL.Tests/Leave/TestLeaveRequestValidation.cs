using LEAVE.BLL.Data;
using LEAVE.BLL.Leave;
using LEAVE.BLL.Security;
using LEAVE.DAL.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LEAVE.BLL.Tests.Leave
{
    [TestClass]
    [TestCategory("Leave Requests")]
    public class TestLeaveRequestValidation
    {
        private List<LeaveRequest> Requests;
        private List<Employee> Employees;
        private ILeaveRequestService leaveRequestService;
        private IDbRepository repository;
        private ISessionService sessionService;

        [TestInitialize]
        public void Initialize()
        {
            repository = GetRepository();
            sessionService = new Mock<ISessionService>().Object;
            leaveRequestService = new LeaveRequestService(repository, sessionService);
        }

        private IDbRepository GetRepository()
        {
            Employees = new List<Employee>
            {
                new Employee
                {
                   Id=1, UserId = "YUU9283923",FirstName ="Amisi", LastName ="Kale", DateOfBirth = DateTime.Today.AddYears(-20),Code ="AMI12", DepartmentId=2, PositionId=1,
                },
                new Employee
                {
                   Id=2, UserId = "UINS283923",FirstName ="John", LastName ="Kale", DateOfBirth = DateTime.Today.AddYears(-22),Code ="AMI13", DepartmentId=2, PositionId=1,
                },
                new Employee
                {
                   Id=3, UserId = "USER92839",FirstName ="Mick", LastName ="Tony", DateOfBirth = DateTime.Today.AddYears(-23),Code ="AMI14", DepartmentId=2, PositionId=1,
                },
                new Employee
                {
                   Id=4, UserId = "ALL938948",FirstName ="Jule", LastName ="Min", DateOfBirth = DateTime.Today.AddYears(-24),Code ="AMI15", DepartmentId=1, PositionId=1,
                }
            };

            Requests = new List<LeaveRequest>
            {
                new LeaveRequest
                {
                    Description = "New Leave",EmployeeId = 1,StartDate = DateTime.Today,EndDate = DateTime.Today.AddDays(7), Id = 1,LeaveTypeId = 2,
                    Employee = Employees.Where(r=>r.Id == 1).FirstOrDefault(),
                },
                new LeaveRequest
                {
                    Description = "New Leave",EmployeeId = 2,StartDate = DateTime.Today,EndDate = DateTime.Today.AddDays(10),Id = 1,LeaveTypeId = 2,
                    Employee = Employees.Where(r=>r.Id == 2).FirstOrDefault(),
                }
            };

            var repoMock = new Mock<IDbRepository>();

            repoMock.Setup(x => x.Set<LeaveRequest>()).Returns(GetQueryableMockDbSet(Requests));
            repoMock.Setup(x => x.Set<Employee>()).Returns(GetQueryableMockDbSet(Employees));

            return repoMock.Object;
        }

        private DbSet<T> GetQueryableMockDbSet<T>(List<T> sourceList) where T : class
        {
            var queryable = sourceList.AsQueryable();

            var dbSet = new Mock<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
            dbSet.Setup(d => d.Add(It.IsAny<T>())).Callback<T>((s) => sourceList.Add(s));

            return dbSet.Object;
        }

        private ISessionService GetSessionService()
        {
            var sessionMock = new Mock<ISessionService>();
            sessionMock.Setup(r => r.IsInSuperRole()).Returns(() => true);
            return sessionMock.Object;
        }

        /// <summary>
        /// We test here the first validation Rule
        /// Employee should not have requests whose dates overlap
        /// </summary>
        [TestMethod]
        public void ThenFailForOverLapingDate()
        {
            var model = new LeaveRequest
            {
                Description = "New Leave",
                EmployeeId = 1,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(6)
            };

            var result = leaveRequestService.ValidateLeaveRequest(model);
            Assert.AreEqual(false, result.Item1);
            Assert.AreEqual("The dates you have selected ovalap with one of your previous requests!", result.Item2);
        }

        [TestMethod]
        public void ThenFailForOverlapingInSameDepartment()
        {
            var model = new LeaveRequest
            {
                Description = "New Leave",
                EmployeeId = 3,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(6)
            };

            var result = leaveRequestService.ValidateLeaveRequest(model);
            Assert.AreEqual(false, result.Item1);
            Assert.AreEqual("The dates you have selected ovalap with onether employee in your department!", result.Item2);
        }

        [TestMethod]
        public void ThenFailForApplicationWithinTheSameMonth()
        {
            var model = new LeaveRequest
            {
                Description = "New Leave",
                EmployeeId = 1,
                StartDate = DateTime.Today.AddDays(8),
                EndDate = DateTime.Today.AddDays(10)
            };

            var result = leaveRequestService.ValidateLeaveRequest(model);
            Assert.IsFalse(result.Item1);
        }

        [TestMethod]
        public void ThenPassForValidApplication()
        {
            var model = new LeaveRequest
            {
                Description = "New Leave",
                EmployeeId = 1,
                StartDate = DateTime.Today.AddDays(37),
                EndDate = DateTime.Today.AddDays(40)
            };

            var result = leaveRequestService.ValidateLeaveRequest(model);
            Assert.IsTrue(result.Item1);
        }
    }
}