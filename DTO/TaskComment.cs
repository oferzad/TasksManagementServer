﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TasksManagementServer.DTO
{
    public class TaskComment
    {
        public int CommentId { get; set; }

        public int? TaskId { get; set; }

        public string Comment { get; set; } = null!;

        public DateOnly CommentDate { get; set; }

        public TaskComment() { }
        public TaskComment(Models.TaskComment modelComment)
        {
            this.CommentId = modelComment.CommentId;
            this.TaskId = modelComment.TaskId;
            this.Comment = modelComment.Comment;
            this.CommentDate = modelComment.CommentDate;
        }
    }
}
