using System;
using System.ComponentModel;
using System.Diagnostics;
using Sample;

class Program
{
    static void Main()
    {
        //GuessGame g = new GuessGame();
        //g.Run();

        //Sudoku s = new Sudoku();
        //s.Run();

        Sudoku9x9 s = new Sudoku9x9();
        s.Run();

        //Tasks2to5 t = new Tasks2to5();
        //t.Run();

        //Tasks6to8 t2 = new Tasks6to8();
        //t2.Run();

        //Task12 t12 = new Task12();
        //t12.Run();
    }
}
