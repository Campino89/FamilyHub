using System;
using System.Collections.Generic;
using System.Text;

namespace FamilyHub.Domain.Meals
{
    public class Meal
    {
        private Meal()
        {
        }

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

        public string Name { get; private set; } = string.Empty;

        private readonly List<MealComponent> _components = new();

        public IReadOnlyCollection<MealComponent> Components => _components;

        public void AddComponent(MealComponent component)
        {
            ArgumentNullException.ThrowIfNull(component);

            if (_components.Any(x => x.Id == component.Id))
                return;

            _components.Add(component);
        }

        public void RemoveComponent(MealComponent component)
        {
            ArgumentNullException.ThrowIfNull(component);

            _components.Remove(component);
        }
    }
}
