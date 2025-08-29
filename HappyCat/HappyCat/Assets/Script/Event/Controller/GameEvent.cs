using HC.Game;
using HC.Scene;

namespace HC.Event
{
    public class GameEvent
    {
        public static EventListeners ServiceEvents = new EventListeners();
    }

    public class JoinCatEvent : IEvent
    {
        public Cat_Actor Cat { get; protected set; }

        public JoinCatEvent(Cat_Actor cat)
        {
            this.Cat = cat;
        }
    }

    public class TableDestinationEvent : IEvent
    {
        public E_DESTINATION State { get; protected set; }
        public Cat_Actor Cat { get; protected set; }
        public int Tableindex { get; protected set; }

        public TableDestinationEvent(Cat_Actor cat, E_DESTINATION state, int index = 0)
        {
            this.Cat = cat;
            this.State = state;
            this.Tableindex = index;
        }
    }

    public class DestinationEvent : IEvent
    {
        public E_DESTINATION State { get; protected set; }
        public Cat_Actor Cat { get; protected set; }
        public DestinationEvent(Cat_Actor cat, E_DESTINATION state)
        {
            this.Cat = cat;
            this.State = state;
        }
    }

    public class StartCookingEvent : IEvent {
        public Food Food { get; protected set; }

        public StartCookingEvent(Food food)
        {
            this.Food = food;
        }
    }

    public class FinishCookingEvent : IEvent
    {
        public FoodData FoodData { get; protected set; }

        public FinishCookingEvent(FoodData FoodData)
        {
            this.FoodData = FoodData;
        }
    }

    public class FinishEatingEvent : IEvent
    {
        public int TableIndex { get; protected set; }

        public FinishEatingEvent(int tableIndex)
        {
            this.TableIndex = tableIndex;
        }
    }
}