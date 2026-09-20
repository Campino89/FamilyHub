using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FamilyHub.Domain.Meals
{
    public class MealPlanEntry
    {
        public MealPlanEntry(DateOnly date, Meal meal)
        {
            ArgumentNullException.ThrowIfNull(meal);

            Datum = date;
            Essen = meal;
        }
        public DateOnly Datum { get; private set; }

        public Meal Essen { get; private set; }
    }
}
