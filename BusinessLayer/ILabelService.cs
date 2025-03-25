using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer;
using RepoLayer;

namespace BusinessLayer
{
    public interface ILabelService
    {
        public List<Label> GetAllLabels();
        public Label GetLabelById(int id);
        public void AddLabel(Label label);
        public void UpdateLabel(int id, Label label);
        public void DeleteLabel(int id);
    }
}