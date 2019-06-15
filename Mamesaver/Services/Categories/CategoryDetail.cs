namespace Mamesaver.Services.Categories
{
    public class CategoryDetail
    {
        public CategoryDetail(string category, string subcategory)
        {
            Category = category;
            Subcategory = subcategory;
        }

        public string Category { get; set; }
        public string Subcategory { get; set; }
    }
}