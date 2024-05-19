namespace Application.Queries
{
    public class GridQuery
    {
        public int Size { get; set; } = 10;
        public int Page { get; set; } = 1;
        public SortModel[] Sorted { get; set; } = new SortModel[] { new SortModel() { column = "CreatedAt", desc = true } };
        public List<FilterModel> Filters { get; set; } = new List<FilterModel>();


        public void RemoveFilter(string filterColumn)
        {
            var filter =
                Filters
                    .Where(filter => filter.column == filterColumn)
                    .FirstOrDefault();

            if (filter != null)
                Filters.Remove(filter);
        }

        public void AddFilter(string column, object value)
        {
            Filters.Add(new FilterModel { column = column, value = value.ToString() });
        }
    }

    public class FilterModel
    {
        public string column { get; set; }
        public string value { get; set; }

        public override string ToString()
        {
            return $"{column} = {value}";
        }
    }

    public class SortModel
    {
        public string column { get; set; }
        public bool desc { get; set; } = false;

        public override string ToString()
        {
            return $"{column} {(desc ? "DESC" : "ASC")}";
        }
    }
}
