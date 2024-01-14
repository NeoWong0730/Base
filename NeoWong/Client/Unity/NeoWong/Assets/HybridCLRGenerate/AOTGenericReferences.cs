using System.Collections.Generic;
public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	public static readonly IReadOnlyList<string> PatchedAOTAssemblyList = new List<string>
	{
		"Google.Protobuf.dll",
		"NWFramework.Runtime.dll",
		"System.Core.dll",
		"UnityEngine.CoreModule.dll",
		"mscorlib.dll",
	};
	// }}

	// {{ constraint implement type
	// }} 

	// {{ AOT generic types
	// Google.Protobuf.IDeepCloneable<object>
	// Google.Protobuf.IMessage<object>
	// Google.Protobuf.MessageParser.<>c__DisplayClass2_0<object>
	// Google.Protobuf.MessageParser<object>
	// NWFramework.IObjectPool<object>
	// NWFramework.NWFrameworkDictionary<int,object>
	// System.Action<byte>
	// System.Action<int,int>
	// System.Action<int>
	// System.Action<object,byte>
	// System.Action<object,int>
	// System.Action<object,object>
	// System.Action<object>
	// System.Action<ulong>
	// System.Collections.Generic.ArraySortHelper<int>
	// System.Collections.Generic.ArraySortHelper<object>
	// System.Collections.Generic.ArraySortHelper<ulong>
	// System.Collections.Generic.Comparer<int>
	// System.Collections.Generic.Comparer<object>
	// System.Collections.Generic.Comparer<ulong>
	// System.Collections.Generic.Dictionary.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection<int,object>
	// System.Collections.Generic.Dictionary.KeyCollection<object,int>
	// System.Collections.Generic.Dictionary.KeyCollection<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,int>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection<int,object>
	// System.Collections.Generic.Dictionary.ValueCollection<object,int>
	// System.Collections.Generic.Dictionary.ValueCollection<object,object>
	// System.Collections.Generic.Dictionary<int,object>
	// System.Collections.Generic.Dictionary<object,int>
	// System.Collections.Generic.Dictionary<object,object>
	// System.Collections.Generic.EqualityComparer<int>
	// System.Collections.Generic.EqualityComparer<object>
	// System.Collections.Generic.HashSet.Enumerator<object>
	// System.Collections.Generic.HashSet<object>
	// System.Collections.Generic.HashSetEqualityComparer<object>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ICollection<byte>
	// System.Collections.Generic.ICollection<int>
	// System.Collections.Generic.ICollection<object>
	// System.Collections.Generic.ICollection<ulong>
	// System.Collections.Generic.IComparer<int>
	// System.Collections.Generic.IComparer<object>
	// System.Collections.Generic.IComparer<ulong>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerable<byte>
	// System.Collections.Generic.IEnumerable<int>
	// System.Collections.Generic.IEnumerable<object>
	// System.Collections.Generic.IEnumerable<ulong>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<int,object>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,int>>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerator<byte>
	// System.Collections.Generic.IEnumerator<int>
	// System.Collections.Generic.IEnumerator<object>
	// System.Collections.Generic.IEnumerator<ulong>
	// System.Collections.Generic.IEqualityComparer<int>
	// System.Collections.Generic.IEqualityComparer<object>
	// System.Collections.Generic.IList<int>
	// System.Collections.Generic.IList<object>
	// System.Collections.Generic.IList<ulong>
	// System.Collections.Generic.KeyValuePair<int,object>
	// System.Collections.Generic.KeyValuePair<object,int>
	// System.Collections.Generic.KeyValuePair<object,object>
	// System.Collections.Generic.List.Enumerator<int>
	// System.Collections.Generic.List.Enumerator<object>
	// System.Collections.Generic.List.Enumerator<ulong>
	// System.Collections.Generic.List<int>
	// System.Collections.Generic.List<object>
	// System.Collections.Generic.List<ulong>
	// System.Collections.Generic.ObjectComparer<int>
	// System.Collections.Generic.ObjectComparer<object>
	// System.Collections.Generic.ObjectComparer<ulong>
	// System.Collections.Generic.ObjectEqualityComparer<int>
	// System.Collections.Generic.ObjectEqualityComparer<object>
	// System.Collections.Generic.Queue.Enumerator<object>
	// System.Collections.Generic.Queue<object>
	// System.Collections.ObjectModel.ReadOnlyCollection<int>
	// System.Collections.ObjectModel.ReadOnlyCollection<object>
	// System.Collections.ObjectModel.ReadOnlyCollection<ulong>
	// System.Comparison<int>
	// System.Comparison<object>
	// System.Comparison<ulong>
	// System.Func<object,byte>
	// System.Func<object,int,int,int,object>
	// System.Func<object,object>
	// System.Func<object>
	// System.IEquatable<object>
	// System.Linq.Buffer<byte>
	// System.Linq.Enumerable.<SkipIterator>d__31<byte>
	// System.Linq.Enumerable.Iterator<object>
	// System.Linq.Enumerable.WhereEnumerableIterator<object>
	// System.Linq.Enumerable.WhereSelectArrayIterator<object,object>
	// System.Linq.Enumerable.WhereSelectEnumerableIterator<object,object>
	// System.Linq.Enumerable.WhereSelectListIterator<object,object>
	// System.Predicate<int>
	// System.Predicate<object>
	// System.Predicate<ulong>
	// System.Runtime.CompilerServices.ConditionalWeakTable.CreateValueCallback<object,object>
	// System.Runtime.CompilerServices.ConditionalWeakTable.Enumerator<object,object>
	// System.Runtime.CompilerServices.ConditionalWeakTable<object,object>
	// UnityEngine.Events.InvokableCall<object>
	// UnityEngine.Events.UnityAction<byte>
	// UnityEngine.Events.UnityAction<object>
	// UnityEngine.Events.UnityEvent<object>
	// }}

	public void RefMethods()
	{
		// object Google.Protobuf.ProtoPreconditions.CheckNotNull<object>(object,string)
		// NWFramework.IObjectPool<object> NWFramework.IObjectPoolManager.CreateSingleSpawnObjectPool<object>(string)
		// object NWFramework.IResourceManager.LoadAsset<object>(string,UnityEngine.Transform,bool,bool,string)
		// object NWFramework.IResourceManager.LoadAsset<object>(string,bool,bool,string)
		// System.Void NWFramework.Log.Error<object>(string,object)
		// System.Void NWFramework.Log.Fatal<object>(string,object)
		// object NWFramework.MemoryPool.Acquire<object>()
		// object NWFramework.MemoryPool.MemoryCollection.Acquire<object>()
		// System.Void NWFramework.NWFrameworkLog.Error<object>(string,object)
		// System.Void NWFramework.NWFrameworkLog.Fatal<object>(string,object)
		// NWFramework.IObjectPool<object> NWFramework.ObjectPoolModule.CreateSingleSpawnObjectPool<object>(string)
		// object NWFramework.ResourceModule.LoadAsset<object>(string,UnityEngine.Transform,bool,bool,string)
		// object NWFramework.ResourceModule.LoadAsset<object>(string,bool,bool,string)
		// System.Void NWFramework.UIBase.AdjustIconNum<object>(System.Collections.Generic.List<object>,int,UnityEngine.Transform,UnityEngine.GameObject,string)
		// object NWFramework.UIBase.CreateWidget<object>(UnityEngine.GameObject,bool)
		// object NWFramework.UIBase.CreateWidgetByPath<object>(UnityEngine.Transform,string,bool)
		// object NWFramework.UIBase.CreateWidgetByPrefab<object>(UnityEngine.GameObject,UnityEngine.Transform,bool)
		// object NWFramework.UIBase.CreateWidgetByType<object>(UnityEngine.Transform,bool)
		// object NWFramework.UIBase.FindChildComponent<object>(UnityEngine.Transform,string)
		// System.Void NWFramework.UIBase.RemoveUnUseItem<object>(System.Collections.Generic.List<object>,int)
		// object NWFramework.UnityExtension.FindChildComponent<object>(UnityEngine.Transform,string)
		// object NWFramework.UnityExtension.GetOrAddComponent<object>(UnityEngine.GameObject)
		// string NWFramework.Utility.Text.Format<object>(string,object)
		// string NWFramework.Utility.Text.ITextHelper.Format<object>(string,object)
		// object System.Activator.CreateInstance<object>()
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Select<object,object>(System.Collections.Generic.IEnumerable<object>,System.Func<object,object>)
		// System.Collections.Generic.IEnumerable<byte> System.Linq.Enumerable.Skip<byte>(System.Collections.Generic.IEnumerable<byte>,int)
		// System.Collections.Generic.IEnumerable<byte> System.Linq.Enumerable.SkipIterator<byte>(System.Collections.Generic.IEnumerable<byte>,int)
		// byte[] System.Linq.Enumerable.ToArray<byte>(System.Collections.Generic.IEnumerable<byte>)
		// System.Collections.Generic.List<object> System.Linq.Enumerable.ToList<object>(System.Collections.Generic.IEnumerable<object>)
		// System.Collections.Generic.IEnumerable<object> System.Linq.Enumerable.Iterator<object>.Select<object>(System.Func<object,object>)
		// object& System.Runtime.CompilerServices.Unsafe.As<object,object>(object&)
		// System.Void* System.Runtime.CompilerServices.Unsafe.AsPointer<object>(object&)
		// object System.Threading.Interlocked.CompareExchange<object>(object&,object,object)
		// object UnityEngine.Component.GetComponent<object>()
		// object UnityEngine.GameObject.AddComponent<object>()
		// object UnityEngine.GameObject.GetComponent<object>()
		// object UnityEngine.Object.Instantiate<object>(object,UnityEngine.Vector3,UnityEngine.Quaternion,UnityEngine.Transform)
	}
}