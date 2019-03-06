using Allure.Commons;
using NUnit.Allure.Attributes;
using NUnit.Allure.Core;
using NUnit.Framework;

namespace NUnitTestProject
{
    public class Tests
    {
        [TestFixture]
        [AllureNUnit]
        [AllureEpic("Allure for C#")]
        [AllureSuite("CalculatorTests")]
        [AllureDisplayIgnored]
        class CalculatorAddTests
        {
            [Test(Description = "Add two integers and returns the sum")]
            [AllureTag("CI")]
            [AllureSeverity(SeverityLevel.critical)]
            [AllureIssue("8911")]
            [AllureTms("Two plus two is four")]
            [AllureFeature("JAMA-CODE This test case verifies that the algorithm makes the correct sum")]
            [AllureOwner("AMC")]
            [AllureSubSuite("Add")]
            public void Return4_WhenAdd2And2()
            {
                int actualResult = 0;
                var calculator = new Calculator();
                AllureLifecycle.Instance.WrapInStep(() =>
                {
                    actualResult = calculator.Add(2, 2);
                }, "Action:sum 2 plus 2");
                AllureLifecycle.Instance.WrapInStep(() => { Assert.AreEqual(4, actualResult);},"Expected 4");
            }
            [Test(Description = "Add two integers and returns the sum")]
            [AllureTag("CI")]
            [AllureSeverity(SeverityLevel.critical)]
            [AllureTms("Zero plus zero is zero")]
            [AllureFeature("JAMA-CODE This test case verifies that the algorithm makes the correct sum")]
            [AllureSubSuite("Add")]
            public void Return0_WhenAdd0And0()
            {
                var calculator = new Calculator();
                int actualResult = 0;
                
                AllureLifecycle.Instance.WrapInStep(() =>
                {
                    actualResult = calculator.Add(0, 0);
                }, "Action:sum 0 plus 0");
                AllureLifecycle.Instance.WrapInStep(() => { Assert.AreEqual(0, actualResult); }, "Expected 0");
            }
            [Test(Description = "Add two integers and returns the sum")]
            [AllureTag("CI")]
            [AllureTms("Zero plus zero is zero Not 1")]
            [AllureFeature("JAMA-CODE This test case verifies that the algorithm makes the correct sum")]
            [AllureSeverity(SeverityLevel.critical)]
            [AllureSubSuite("Add")]
            public void ReturnMinus5_WhenAddMinus3AndMinus2()
            {
                var calculator = new Calculator();
                int actualResult = 0;
               
                AllureLifecycle.Instance.WrapInStep(() =>
                {
                    actualResult = calculator.Add(0, 0);
                }, "Action:sum 0 plus 0");
                AllureLifecycle.Instance.WrapInStep(() => { Assert.AreNotEqual(1, actualResult); Assert.AreEqual(0, actualResult); }, "Not equal to 1, equal to 0");
                AllureLifecycle.Instance.WrapInStep(() =>
                {
                    actualResult = calculator.Add(5, 0);
                }, "Action:sum 5 plus 0");
                AllureLifecycle.Instance.WrapInStep(() => { Assert.AreNotEqual(1, actualResult);
                    Assert.AreEqual(5, actualResult);
                }, "Not equal to 1, equal to 5");
            }
        }
    }
}