using System.Collections.Generic;

namespace Week5Lesson25
{
    class GroupsHelper
    {
        public static List<Group> GetGroups(string name)
        {
            return new List<Group>
            {
                new Group { Id = 0, Name = name },
                new Group { Id = 1, Name = "A1" },
                new Group { Id = 2, Name = "A2" },
                new Group { Id = 3, Name = "A3" }
            };        
        }
    }
}
