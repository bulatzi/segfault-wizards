using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace AbetApi.EFModels
{
    public class Grade
    {
        [JsonIgnore]
        public int GradeId { get; set; }
        public string Major { get; set; }

        public int A { get; set; }
        public int B { get; set; }
        public int C { get; set; }
        public int D { get; set; }
        public int F { get; set; }
        public int W { get; set; }
        public int I { get; set; }
        public int TotalStudents { get; set; }

        Grade()
        {
            // Intentionally left blank
        }

        Grade(string Major, int A, int B, int C, int D, int F, int W, int I, int TotalStudents)
        {
            this.Major = Major;
            this.A = A;
            this.B = B;
            this.C = C;
            this.D = D;
            this.F = F;
            this.W = W;
            this.I = I;
            this.TotalStudents = TotalStudents;
        }
    }
}
