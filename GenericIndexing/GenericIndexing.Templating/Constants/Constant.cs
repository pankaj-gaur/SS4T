
namespace GenericIndexing.Templating
{
    /// <summary>
    /// Constants
    /// </summary>
    public class Constant
    {

        public const string LocationXmlPackageVariableName = "Xml";

        public const string Field_Name = "PlaceHolderContent";
        public const string Component_Template = "ComponentTemplate";
        public const string Component_Template_Column = "CT_Container";
        public const string ContainerOne = "Container 1";
        public const string ContainerTwo = "Container 2";
        public const string ContainerThree = "Container 3";
        public const string ContainerFour = "Container 4";

        public class MegaMenu
        {
            public const string Name = "megamenu";
            public const string CustomLink = "customlink";
            public const string Title = "title";
            public const string ShowCategory = "showcategory";
            public const string ShowSubCategory = "showsubcategory";
            public const string PromotionSlot = "promotionslots";
            public const string Video = "video";
            public class MenuItems
            {
                public const string Name = "menuitems";
                public const string Title = "title";
                public const string PageLink = "pagelink";
            }
            public class Link
            {
                public const string InternalLink = "internallink";
                public const string ExternalLink = "externallink";
                public const string Href = "xlink:href";
                public const string UrlType = "urltype";
                public const string XLinkTitle = "xlink:title";
                public const string HashCode = "anchor";
                public const string OpenInWhichWindow = "openinwhichwindow";
            }

        }

        /// <summary>
        /// Location xml constant
        /// </summary>
        public class LocationXmlStructure
        {
            public class Root
            {
                public const string Name = "root";

                public class Locale
                {
                    public const string Name = "locale";
                    public const string A_Location = "location";
                    public const string A_DisplayName = "displayname";
                    public const string A_DefaultLanguage = "defaultlanguage";
                    public const string A_ParentKeyword = "parentkeyword";
                    public const string A_KeywordValue = "keywordvalue";
                    public const string A_DefaultVrtical = "defaultvertical";
                }

                public class Languages
                {
                    public const string Name = "languages";
                    public class Language
                    {
                        public const string Name = "language";
                        public const string A_Name = "name";
                        public const string A_DisplayName = "displayname";
                        public const string A_Value = "key";
                    }
                }
                public class Regions
                {
                    public const string Name = "regions";
                }
            }
        }

        public class CategoryXmlStructure 
        {
            public const string Root = "categories";

            public class Category
            {
                public const string Name = "category";
                public const string Name_A = "name";
                public const string ID_A = "id";


                public class Keywords
                {
                    public const string Name = "keywords";

                    public const string Metadata = "metadata";

                    public class Keyword
                    {
                        public const string Name = "keyword";
                        public const string Title_A = "title";
                        public const string Key_A = "key";
                        public const string DEscription_A = "decription";
                    }
                }
            }
        }

    }
}
