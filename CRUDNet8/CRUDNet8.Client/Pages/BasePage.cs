using Microsoft.AspNetCore.Components;
using SharedLibrary.Models;

namespace CRUDNet8.Client.Pages;

// https://www.youtube.com/watch?v=qQ91rdMgIMM
public class BasePage : ComponentBase, IDisposable
{
    [Inject]
    public PersistentComponentState ApplicationState { get; set; }

    private readonly IList<PersistingComponentStateSubscription> subscriptions = new List<PersistingComponentStateSubscription>();

    protected async Task<TResult?> GetOrAddState<TResult>(string key, Func<Task<TResult?>> addStateFactory)
    {
        TResult? data = default;

        subscriptions.Add(ApplicationState.RegisterOnPersisting(() =>
        {
            ApplicationState.PersistAsJson(key, data);
            return Task.CompletedTask;
        }));

        if (ApplicationState.TryTakeFromJson(key, out TResult? storedData))
        {
            data = storedData;
        }
        else
        {
            data = await addStateFactory.Invoke();
        }

        return data;
    }

    public void Dispose()
    {
        if (subscriptions.Count > 0)
        {
            foreach (var sub in subscriptions)
            {
                sub.Dispose();
            }
        }
    }
}
