using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FlughafenManagerTool
{
    public class Controller
    {
        private List<Data> data = new List<Data>();
        private List<Data> backup = null;

        public List<Data> GetData()
        {
            return this.data.ToList();
        }

        public void AddItem(Data item)
        {
            this.data.Add(item);

            if (this.backup != null)
                this.backup.Add(item);
        }

        public void RemoveItem(int pos)
        {
            if (this.backup != null)
            {
                Data backData = this.backup[pos];
                this.data.Remove(backData);
                this.backup.RemoveAt(pos);
            }
            else
                this.data.RemoveAt(pos);
        }

        public void ItemChanged(Data item, int index)
        {
            if (this.backup != null)
            {
                Data backData = this.backup[index];
                this.data[this.data.IndexOf(backData)] = item;
                this.backup[index] = item;
            }
            else
                this.data[index] = item;
        }

        public List<Data> SearchList(string search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                if (this.backup != null)
                    this.backup = null;
            }
            else
            {
                this.backup = this.data.Where(x => x.Name.ToLower().Contains(search.ToLower()) || x.Id.ToLower().Contains(search.ToLower())).ToList();
                return this.backup.ToList();
            }

            return this.data.ToList();
        }

        public List<Data> SortList()
        {
            if (this.backup != null)
            {
                this.data = this.data.OrderBy(x => x.Name).ToList();
                this.backup = this.backup.OrderBy(x => x.Name).ToList();
                return this.backup.ToList();
            }
            else
            {
                if (this.data.Count >=2)
                    this.data = this.data.OrderBy(x => x.Name).ToList();

                return this.data.ToList();
            }
        }

        public void WriteCsv(string filename)
        {
            using (StreamWriter writer = new StreamWriter(filename))
            {
                foreach (Data line in this.data)
                {
                    writer.WriteLine($"{line.Name};{line.Id};{line.Country}");
                }
            }
        }

        public List<Data> ReadCsv(string filename)
        {
            this.data = new List<Data>();

            using (StreamReader reader = new StreamReader(filename))
            {
                while (!reader.EndOfStream)
                {
                    string[] line = reader.ReadLine().Split(';');
                    if (line.Length == 3)
                        this.data.Add(new Data() { Name = line[0], Id = line[1], Country = line[2] });
                }
            }

            return this.data.ToList();
        }
    }
}
