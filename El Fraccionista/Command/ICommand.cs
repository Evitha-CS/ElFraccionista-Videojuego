using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Command
{
    public interface ICommand
    {
        Task Execute();
    }

}

