using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using App.CoreLib.EF;
using App.CoreLib.EF.Context;
using Microsoft.Extensions.DependencyInjection;

namespace App.CoreLib.IntegrationTest
{
    [TestFixture]
    public class IntegrationTestBase
    {
        protected IServiceCollection Services;

        protected IServiceProvider ServiceProvider;
        private IntegrationTestFactory contextFactory;
        protected IStorage context;
        public delegate void TestMethod();

        public IntegrationTestBase()
        {
        }

        [OneTimeSetUp]
        public void Setup()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.User = new GenericPrincipal(new ClaimsIdentity(GetUserClaims()), new string[0]);

            Globals.SetHttpContext(httpContext);

            contextFactory = new IntegrationTestFactory();

            var dbContext = contextFactory.CreateDbContext(new string[] { });
            ServiceProvider = contextFactory.Services.BuildServiceProvider();
            var eventHandler = ServiceProvider.GetRequiredService<EF.Events.EventHandlerContainer>();
            context = new Storage(dbContext, eventHandler);
            TestInitialize();
        }

        private IEnumerable<Claim> GetUserClaims()
        {
            List<Claim> claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.NameIdentifier, "NamL"));
            claims.Add(new Claim(ClaimTypes.Name, "Nam Le"));
            return claims;
        }

        public virtual void TestInitialize()
        {

        }
        protected virtual void RunBeforeTest()
        {
        }

        protected virtual void RunAfterTest()
        {
        }

        public virtual async Task RunTestAsync(Func<Task> testMethod, bool doCommit = false)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                this.RunBeforeTest();

                await testMethod();

                this.RunAfterTest();
                if (doCommit) scope.Complete();
            }
        }

        public virtual void RunTest(TestMethod testMethod, bool doCommit = false)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                this.RunBeforeTest();

                testMethod();

                this.RunAfterTest();
                if (doCommit) scope.Complete();
            }
        }
    }
}