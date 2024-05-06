
using swas.DAL;
using swas.UI.Controllers;
using Moq;
using swas.BAL.Interfaces;
using swas.BAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace swasUnitTest
{
    [TestClass]
    public class SoftwareTypeControllerUnitTest
    { //A: Arrangement
        SoftwareType s1;
        SoftwareType s2;
        SoftwareType s3;
        List<SoftwareType> SoftwareTypeList ;
        SoftwareTypeController ctr1;
        Mock<ISoftwareTypeRepository> uow;
        public SoftwareTypeControllerUnitTest()
        {
            s1 = new SoftwareType { Id =1,TypeOfSoftware="game"};
            s2 = new SoftwareType { Id = 2, TypeOfSoftware = "face Recognition" };
            s3 = new SoftwareType { Id = 3, TypeOfSoftware = "TouchPaid" };

            SoftwareTypeList = new List<SoftwareType>();
            SoftwareTypeList.Add(s1);
            SoftwareTypeList.Add(s2);
            SoftwareTypeList.Add(s3);
            uow = new Mock<ISoftwareTypeRepository>();
            ctr1 = new SoftwareTypeController(uow.Object);
        }

        [TestMethod]
        public void TestList()
        {


       // //     setup

       //     uow.Setup(u => u._SoftwareRepo.GetAll().Returns(SoftwareTypeList));

       // // A: Action

       //     var result = ctr1.List() as ViewResult;
       //     var model = ViewResult.Model as List<SoftwareType>;

       // // A: Assert
       //  //Positive Test Case



       //     CollectionAssert.Contains(model, s1);
       //     CollectionAssert.Contains(model, s2);



       //  //    failed
       //           CollectionAssert.Contains(model, s6);

       //   //   does not faileda
       //   //   Negative Test Case
       //          CollectionAssert.DoesNotContain(model, s6);

        }
    }
}
