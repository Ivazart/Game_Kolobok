using System;

namespace PatchProcessing
{
    [Serializable]
    public class Patch
    {
        public string action;
        public Arguments arguments;
        public string _comment; // игнорируется при выполнении
    }

    [Serializable]
    public class Arguments
    {
        public string file_path;
        public string file_content;
        public string search;
        public string replace;
        public object occurrence; // строка "all" или число
        public string directory;
        public string source_path;  // для move_file
        public string destination_path; // для move_file
        public string class_name;        // для insert_before_class_end (опционально)
        public string search_line_contains; // для insert_after_line
    }
}
