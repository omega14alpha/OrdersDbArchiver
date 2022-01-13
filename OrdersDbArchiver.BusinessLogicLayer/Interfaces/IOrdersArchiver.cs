namespace OrdersDbArchiver.BusinessLogicLayer.Interfaces
{
    public interface IOrdersArchiver
    {
        void Start();

        void StopWork();
    }
}
