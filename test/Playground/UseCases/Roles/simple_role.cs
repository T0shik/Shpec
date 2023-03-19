using Playground.UseCases.Roles.SimpleRoles;
using Shpec;

namespace Playground.UseCases.Roles
{
    public class simple_role : IUseCase
    {
        public void Execute()
        {
            var actor = new Actor()
            {
                FirstName = "Bob",
                LastName = "Martin",
            };
            var introduction = new Introduction(actor);
            introduction.Do();
        }
    }
}

namespace Playground.UseCases.Roles.SimpleRoles
{
    public partial class Actor
    {
        private Members _m => new(
            Property.FirstName,
            Property.LastName
        );
    }

    public partial class Introduction
    {
        public Introduction(Actor actor)
        {
            Speaker = actor;
        }

        public partial class SpeakerRole
        {
            private Role _r => new Role(
                Property.FirstName,
                Property.LastName
            );

            public string IntroduceThySelf()
            {
                return $"Hello my name is {FirstName} {LastName}, and I like TDD.";
            }
        }

        public void Do()
        {
            var introduction = Speaker.IntroduceThySelf();
            Console.WriteLine(introduction);
        }
    }
}