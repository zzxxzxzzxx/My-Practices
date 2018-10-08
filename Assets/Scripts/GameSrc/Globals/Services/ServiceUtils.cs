/// File create date:10/8/2018
using RoachGame.Services;
// Created By Yu.Liu
public class ServiceUtils {
    public static EventService GetEventService() {
        return (ServiceProvider.Instance.ProvideService(GameConfigs.TAG_SVC_EVENT) as EventService);
    }
}
