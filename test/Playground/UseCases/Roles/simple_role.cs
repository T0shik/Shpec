using System.Diagnostics;
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
            Debug.Assert(introduction.Speaker == introduction.Reflection, "object identity is destroyed");
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
            Reflection = actor;
        }

        public partial interface ISpeaker
        {
            private Members _r => new Members(
                Property.FirstName,
                Property.LastName
            );

            public void IntroduceThySelf()
            {
                Console.WriteLine($"Self: Hello my name is {FirstName} {LastName}, and I like TDD.");
                Context.Reflection.IntroduceThee();
            }
        }
        
        public partial interface IReflection
        {
            private Members _r => new Members(
                Property.FirstName,
                Property.LastName
            );

            public void IntroduceThee()
            {
                Console.WriteLine($"Reflection: You are {FirstName} {LastName}, and you like TDD.");
            }
        }

        public void Do()
        {
            Speaker.IntroduceThySelf();
        }
    }
}