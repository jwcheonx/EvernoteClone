using System;
using System.Collections.Generic;
using System.IO;
using EvernoteClone.Models;
using Microsoft.Data.Sqlite;

namespace EvernoteClone.ViewModels.Helpers
{
    internal static class DatabaseHelper
    {
        public static readonly string DatabaseDirname = Directory.CreateDirectory(Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "EvernoteClone"
        )).FullName;

        private static readonly string s_baseConnectionString =
            $"Data Source={Path.Combine(DatabaseDirname, "notes.sqlite")};Foreign Keys=True";

        public static void InitializeDatabase()
        {
            using SqliteConnection conn = new (s_baseConnectionString);
            conn.Open();

            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = """
                              /* Enable write-ahead logging. */
                              PRAGMA journal_mode=WAL;
                              PRAGMA synchronous=NORMAL;

                              CREATE TABLE IF NOT EXISTS user(
                                  id INTEGER PRIMARY KEY,
                                  first_name TEXT NOT NULL,
                                  last_name TEXT NOT NULL,
                                  username TEXT NOT NULL,
                                  password TEXT NOT NULL
                              );

                              CREATE TABLE IF NOT EXISTS notebook(
                                  id INTEGER PRIMARY KEY,
                                  user_id INTEGER REFERENCES user ON DELETE CASCADE,
                                  title TEXT DEFAULT 'Untitled'
                              );

                              CREATE TABLE IF NOT EXISTS note(
                                  id INTEGER PRIMARY KEY,
                                  notebook_id INTEGER NOT NULL REFERENCES notebook ON DELETE CASCADE,
                                  title TEXT DEFAULT (strftime('%m/%d/%Y %H:%M:%S', 'now', 'localtime')),
                                  /* Store Unix timestamps to save storage. */
                                  created_at INTEGER DEFAULT (unixepoch()),
                                  updated_at INTEGER DEFAULT (unixepoch()),
                                  filename TEXT
                              );
                              """;

            cmd.ExecuteNonQuery();
        }

        public static IEnumerable<Notebook>? GetNotebooks()
        {
            using SqliteConnection conn = new ($"{s_baseConnectionString};Mode=ReadOnly");
            conn.Open();

            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM notebook;";

            using SqliteDataReader reader = cmd.ExecuteReader();
            if (!reader.HasRows)
            {
                return null;
            }

            List<Notebook> notebooks = [];
            while (reader.Read())
            {
                notebooks.Add(new Notebook(
                    reader.GetInt32(0),
                    null,
                    reader.GetString(2)
                ));
            }

            return notebooks;
        }

        public static Notebook? AddNotebook()
        {
            using SqliteConnection conn = new ($"{s_baseConnectionString};Mode=ReadWrite");
            conn.Open();

            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = """
                              INSERT INTO notebook
                              DEFAULT VALUES
                              RETURNING *;
                              """;

            using SqliteDataReader reader = cmd.ExecuteReader();
            if (!reader.HasRows)
            {
                return null;
            }

            reader.Read();
            return new Notebook(
                reader.GetInt32(0),
                null,
                reader.GetString(2)
            );
        }

        public static bool RenameNotebook(int id, string newTitle)
        {
            using SqliteConnection conn = new ($"{s_baseConnectionString};Mode=ReadWrite");
            conn.Open();

            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = """
                              UPDATE notebook
                              SET title = $new_title
                              WHERE id = $id;
                              """;
            cmd.Parameters.AddWithValue("$id", id);
            cmd.Parameters.AddWithValue("$new_title", newTitle);

            return cmd.ExecuteNonQuery() > 0;
        }

        public static bool DeleteNotebook(int id)
        {
            using SqliteConnection conn = new ($"{s_baseConnectionString};Mode=ReadWrite");
            conn.Open();

            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = """
                              DELETE FROM notebook
                              WHERE id = $id;
                              """;
            cmd.Parameters.AddWithValue("$id", id);

            return cmd.ExecuteNonQuery() > 0;
        }

        public static IEnumerable<Note>? GetNotes(int notebookId)
        {
            using SqliteConnection conn = new ($"{s_baseConnectionString};Mode=ReadOnly");
            conn.Open();

            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = """
                              SELECT * FROM note
                              WHERE notebook_id = $notebook_id
                              ORDER BY updated_at;
                              """;
            cmd.Parameters.AddWithValue("$notebook_id", notebookId);

            using SqliteDataReader reader = cmd.ExecuteReader();
            if (!reader.HasRows)
            {
                return null;
            }

            List<Note> notes = [];
            while (reader.Read())
            {
                Note note = new (
                    reader.GetInt32(0),
                    reader.GetInt32(1),
                    reader.GetString(2),
                    reader.GetInt64(3),
                    reader.GetInt64(4)
                );
                if (!reader.IsDBNull(5))
                {
                    note.Filename = reader.GetString(5);
                }

                notes.Add(note);
            }

            return notes;
        }

        public static Note? AddNote(int notebookId)
        {
            using SqliteConnection conn = new ($"{s_baseConnectionString};Mode=ReadWrite");
            conn.Open();

            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = """
                              INSERT INTO note(notebook_id)
                              VALUES ($notebook_id)
                              RETURNING
                                id,
                                notebook_id,
                                title,
                                created_at,
                                updated_at;
                              """;
            cmd.Parameters.AddWithValue("$notebook_id", notebookId);

            using SqliteDataReader reader = cmd.ExecuteReader();
            if (!reader.HasRows)
            {
                return null;
            }

            reader.Read();
            return new Note(
                reader.GetInt32(0),
                reader.GetInt32(1),
                reader.GetString(2),
                reader.GetInt64(3),
                reader.GetInt64(4)
            );
        }

        public static bool UpdateNote(Note note)
        {
            using SqliteConnection conn = new ($"{s_baseConnectionString};Mode=ReadWrite");
            conn.Open();

            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = """
                              UPDATE note
                              SET filename = $filename, updated_at = (unixepoch())
                              WHERE id = $id
                              RETURNING updated_at;
                              """;
            cmd.Parameters.AddWithValue("$id", note.Id);
            // TODO: Handle exceptions.
            cmd.Parameters.AddWithValue("$filename", note.Filename ??= Path.Combine(
                Directory
                    .CreateDirectory(Path.Combine(DatabaseDirname, $"rtf\\{note.NotebookId}"))
                    .FullName,
                $"{note.Id}.rtf"
            ));

            using SqliteDataReader reader = cmd.ExecuteReader();
            if (!reader.HasRows)
            {
                return false;
            }

            reader.Read();
            note.UpdatedAt = reader.GetInt64(0);

            return true;
        }

        public static bool DeleteNote(int id)
        {
            using SqliteConnection conn = new ($"{s_baseConnectionString};Mode=ReadWrite");
            conn.Open();

            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = """
                              DELETE FROM note
                              WHERE id = $id;
                              """;
            cmd.Parameters.AddWithValue("$id", id);

            return cmd.ExecuteNonQuery() > 0;
        }
    }
}
