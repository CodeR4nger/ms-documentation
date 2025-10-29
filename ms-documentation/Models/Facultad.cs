namespace ms_documentation.Models;

    public class Facultad {
        public int Id { get; set; }
        public required string Nombre { get; set;}
        public required string Ciudad { get; set;}
        public required Universidad Universidad { get; set;}
    }