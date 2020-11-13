using System.Linq;
using UnityEngine;

namespace Core.OrderStart
{
    /// <summary>
    /// Контроллер последовательного запуска скриптов.
    /// Все MonoBehaviour'ы, которые мы хотим включить в последовательную зхагрузку,
    /// должны наследоваться от интерфейса IStarter, и иметь функцию OnStart.
    /// Эта функция будет вызываться у каждого MonoBehaviour один за другим в той последовательности,
    /// в которой они указаны в инспекторе
    /// </summary>
    public class OrderStarter: MonoBehaviour
    {
        [Header("Набор скриптов последовательной загрузки")]
        [SerializeField] private MonoBehaviour[] startableBehaviours;
        
        private void Start()
        {
            foreach (var behaviour in startableBehaviours.OfType<IStarter>())
                behaviour.OnStart();
        }
    }
}