using System;
using System.Collections.Generic;
using System.Text;

namespace FamilyHub.Domain.Meals
{
    public class Meal
    {
        public Meal(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Der Name eines Gerichts darf nicht leer sein.", nameof(name));
            }

            Id = Guid.NewGuid();
            Name = name;
        }

        public Guid Id { get; private set; }

        public string Name { get; private set; }
    }
}
