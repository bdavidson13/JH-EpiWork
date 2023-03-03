using System.Web;
using System.Web.Mvc;
using AlloyDemo.IdProviders;
using AlloyDemo.Managers;
using AlloyDemo.Models.Pages;
using AlloyDemo.Models.ViewModels;
using AlloyDemo.Stores;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Mvc;

namespace AlloyDemo.Controllers
{
    public class StartPageController : PageControllerBase<StartPage>
    {
        private readonly IProfileStore _store;
        private readonly IIdProvider _idProvider;
        public StartPageController(IProfileStore store, IIdProvider idProvider)
        {
            _store = store;
            _idProvider = idProvider;
        }
        
        public ActionResult Index(StartPage currentPage)
        {
            var model = PageViewModel.Create(currentPage);
            var profileManger = new ProfileManager(_store, _idProvider, this.HttpContext);

            if (SiteDefinition.Current.StartPage.CompareToIgnoreWorkID(currentPage.ContentLink)) // Check if it is the StartPage or just a page of the StartPage type.
            {
                //Connect the view models logotype property to the start page's to make it editable
                var editHints = ViewData.GetEditHints<PageViewModel<StartPage>, StartPage>();
                editHints.AddConnection(m => m.Layout.Logotype, p => p.SiteLogotype);
                editHints.AddConnection(m => m.Layout.ProductPages, p => p.ProductPageLinks);
                editHints.AddConnection(m => m.Layout.CompanyInformationPages, p => p.CompanyInformationPageLinks);
                editHints.AddConnection(m => m.Layout.NewsPages, p => p.NewsPageLinks);
                editHints.AddConnection(m => m.Layout.CustomerZonePages, p => p.CustomerZonePageLinks);
            }

            return View(model);
        }

    }
}
