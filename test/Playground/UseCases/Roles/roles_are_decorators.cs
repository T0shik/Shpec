using System.Diagnostics;
using Playground.UseCases.Roles.Decorators;
using Shpec;

namespace Playground.UseCases.Roles
{
    public class roles_are_decorators : IUseCase
    {
        public void Execute()
        {
            var actor = new Actor() { Age = 10 };
            var hb = new HappyBirthday(actor);
            hb.Do();

            Debug.Assert(actor.Age == 11, "expected age to be incremented from 10 to 11");
        }
    }
}

namespace Playground.UseCases.Roles.Decorators
{
    public partial class Actor
    {
        private Members _m => new(
            Property.Age
        );
    }

    public partial class HappyBirthday
    {
        public HappyBirthday(Actor actor)
        {
            BirthdayPerson = actor;
        }

        public partial class BirthdayPersonRole
        {
            private Role _r => new Role(
                Property.Age
            );

            public void IncrementAge()
            {
                Age++;
            }
        }

        public void Do()
        {
            BirthdayPerson.IncrementAge();
        }
    }
}