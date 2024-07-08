using BAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using swas.BAL;


namespace swasUnitTest
{
    
    [TestClass]
    public class SoftwareUnitTest
    {
        //A : Arrangement
        Calculator calc;
        public SoftwareUnitTest()
        {
            calc = new Calculator();
        }
        [TestMethod]
        public void TestAdd()
        {
            int num1 =3 , num2 =3 ;
            //A : Action
            int result= calc.Sum(num1 ,num2);

            //A :Assertion
            Assert.AreEqual(num1+num2,result);
        }
    }
}