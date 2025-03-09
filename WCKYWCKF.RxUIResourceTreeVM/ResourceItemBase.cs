using System.Collections.ObjectModel;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;

namespace WCKYWCKF.RxUIResourceTreeVM;

public abstract class ResourceItemBase : ReactiveObject
{
    protected ResourceItemBase(ResourceItemRootBase root)
    {
        Root = root;
        // ReSharper disable once VirtualMemberCallInConstructor
        Children = CreateChildren();
        this.WhenAnyValue(x => x.IsExpanded)
            .Where(x => x && IsLoaded is false)
            .Do(_ =>
            {
                _children.Clear();
                LoadChildren();
                IsLoaded = true;
            })
            .Subscribe();

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (root is null) return;
        Children.ToObservableChangeSet(x => x.Id)
            .OnItemAdded(x => x.Parent = this)
            .OnItemRemoved(x => x.Parent = null)
            .OnItemUpdated((newValue, oldValue) =>
            {
                newValue.Parent = this;
                oldValue.Parent = null;
            })
            .OnItemAdded(Root.OnResourceItemAdded)
            .OnItemRemoved(Root.OnResourceItemRemoved)
            .OnItemRefreshed(Root.OnResourceItemPropertyChanged)
            .Subscribe();
    }

    internal abstract ObservableCollection<ResourceItemBase> _children { get; }

    public string? Header
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public bool IsExpanded
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public bool IsLoaded
    {
        get;
        protected set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public bool IsSelected
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public bool IsRoot => Parent is null;

    public ReadOnlyObservableCollection<ResourceItemBase> Children { get; }

    public ResourceItemBase? Parent
    {
        get;
        private set => this.RaiseAndSetIfChanged(ref field, value);
    }

    public Guid Id { get; } = Guid.NewGuid();

    public ResourceItemRootBase Root { get; }

    public static ResourceItemBase Empty { get; } = new EmptyResourceItemBase(null!);

    protected abstract ReadOnlyObservableCollection<ResourceItemBase> CreateChildren();

    protected abstract void LoadChildren();

    private class EmptyResourceItemBase : ResourceItemBase
    {
        public EmptyResourceItemBase(ResourceItemRootBase root) : base(root)
        {
        }

        internal override ObservableCollection<ResourceItemBase> _children { get; } = [];

        protected override ReadOnlyObservableCollection<ResourceItemBase> CreateChildren()
        {
            return new ReadOnlyObservableCollection<ResourceItemBase>(_children);
        }

        protected override void LoadChildren()
        {
            throw new NotSupportedException("LoadChildren is not supported.this class should never be called.");
        }
    }
}