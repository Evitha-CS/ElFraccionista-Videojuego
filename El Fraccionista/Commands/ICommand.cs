using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Commands
{
    /*
     * Esta interfaz se debe implementar en las clases que serán utilzadas como comandos
     */
    public interface ICommand
    {
        Task Execute();
    }
}

