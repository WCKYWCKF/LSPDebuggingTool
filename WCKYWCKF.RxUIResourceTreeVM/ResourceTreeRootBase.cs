using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace WCKYWCKF.RxUIResourceTreeVM;

public abstract class ResourceItemRootBase
{
    protected ResourceItemRootBase()
    {
        OnItemAdded = Observable.FromEvent<Action<ResourceItemBase>, ResourceItemBase>(
            x => OnItemAddedEvent += x,
            x => OnItemAddedEvent -= x);
        OnItemRemoved = Observable.FromEvent<Action<ResourceItemBase>, ResourceItemBase>(
            x => OnItemRemovedEvent += x,
            x => OnItemRemovedEvent -= x);
        OnItemRefreshed = Observable.FromEvent<Action<ResourceItemBase>, ResourceItemBase>(
            x => OnItemRefreshedEvent += x,
            x => OnItemRefreshedEvent -= x);
    }

    public IObservable<ResourceItemBase> OnItemAdded { get; }
    public IObservable<ResourceItemBase> OnItemRemoved { get; }
    public IObservable<ResourceItemBase> OnItemRefreshed { get; }


    public abstract ReadOnlyObservableCollection<ResourceItemBase> Items { get; }
    private event Action<ResourceItemBase>? OnItemAddedEvent;
    private event Action<ResourceItemBase>? OnItemRemovedEvent;
    private event Action<ResourceItemBase>? OnItemRefreshedEvent;

    internal void OnResourceItemAdded(ResourceItemBase resourceItemBase)
    {
        OnItemAddedEvent?.Invoke(resourceItemBase);
    }

    internal void OnResourceItemPropertyChanged(ResourceItemBase resourceItemBase)
    {
        OnItemRefreshedEvent?.Invoke(resourceItemBase);
    }

    internal void OnResourceItemRemoved(ResourceItemBase resourceItemBase)
    {
        OnItemRemovedEvent?.Invoke(resourceItemBase);
    }
}