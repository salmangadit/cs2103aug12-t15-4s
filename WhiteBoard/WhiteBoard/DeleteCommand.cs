using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace WhiteBoard
{
    class DeleteCommand : Command
    {
        List<int> taskIdsToDelete;
        List<Task> tasksToDelete;

        public DeleteCommand(FileHandler fileHandler, int taskIdToDelete, List<Task> screenState)
            : base(fileHandler, screenState)
        {
            taskIdsToDelete = new List<int>();
            tasksToDelete = new List<Task>();
            this.taskIdsToDelete.Add(taskIdToDelete);
            this.commandType = CommandType.Delete;
        }

        public DeleteCommand(FileHandler fileHandler, List<int> taskIdsToDelete, List<Task> screenState)
            : base(fileHandler, screenState)
        {
            taskIdsToDelete = new List<int>();
            tasksToDelete = new List<Task>();
            this.taskIdsToDelete = taskIdsToDelete;
            this.commandType = CommandType.Delete;
        }

        public override CommandType CommandType
        {
            get
            {
                return commandType;
            }
        }

        public override List<Task> Execute()
        {
            foreach (int taskIdToDelete in taskIdsToDelete)
            {
                tasksToDelete.Add(fileHandler.GetTaskFromFile(taskIdToDelete));
                bool isTaskDeleted = fileHandler.DeleteTaskFromFile(taskIdToDelete);

                if (!isTaskDeleted)
                    throw new ApplicationException("Unable To Delete Task with ID" + taskIdToDelete);
            }

            return tasksToDelete;
        }

        public override List<Task> Undo()
        {
            foreach (Task task in tasksToDelete)
            {
                fileHandler.AddTaskToFile(task, task.Id);
            }
            return screenState;
        }

        public List<int> GetDeletedTaskId()
        {
            return taskIdsToDelete;
        }
    }
}
