using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ModelLayer;
using RepoLayer;

namespace BusinessLayer
{
    public class LabelService : ILabelService
    {
        private readonly UserDBContext context;

        public LabelService(UserDBContext context)
        {
            this.context = context;
        }

        public void AddLabel(Label label)
        {
            context.Labels.Add(label);
            context.SaveChanges();
        }

        public void DeleteLabel(int id)
        {
            var label = context.Labels.Find(id);
            if (label == null)
            {
                throw new KeyNotFoundException("Label not found!");
            }
            context.Labels.Remove(label);
            context.SaveChanges();
        }

        public List<Label> GetAllLabels()
        {
            return context.Labels.ToList();
        }

        public Label GetLabelById(int id)
        {
            var label = context.Labels.Find(id);
            if (label == null)
            {
                throw new KeyNotFoundException("Label not found!");
            }
            return label;
        }

        public void UpdateLabel(int id, Label label)
        {
            var existingLabel = context.Labels.Find(id);
            if (existingLabel == null)
            {
                throw new KeyNotFoundException("Label not found!");
            }
            existingLabel.Name = label.Name;
            context.Labels.Update(existingLabel);
            context.SaveChanges();
        }
    }
}