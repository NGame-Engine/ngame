using System.Collections.ObjectModel;
using System.Reactive;
using System.Windows.Input;
using DynamicData.Binding;
using NGameEditor.Results;

namespace NGameEditor.ViewModels.ProjectWindows.HierarchyViews;



public class HierarchyViewModel(
	ReactiveCommand<Unit, Result> addEntityCommand
) : ViewModelBase
{
	private string _searchFilter = "";


	public string SearchFilter
	{
		get => _searchFilter;
		set => this.RaiseAndSetIfChanged(ref _searchFilter, value);
	}

	public ObservableCollectionExtended<EntityNodeViewModel> SceneEntities { get; set; } = new();


	public ObservableAsPropertyHelper<IEnumerable<EntityNodeViewModel>> SearchResultsHelper { get; set; } = null!;
	public IEnumerable<EntityNodeViewModel> SearchResults => SearchResultsHelper.Value;

	public ObservableCollection<EntityNodeViewModel> SelectedEntities { get; set; } = new();


	public ICommand? AddEntity { get; set; } = addEntityCommand;
}
