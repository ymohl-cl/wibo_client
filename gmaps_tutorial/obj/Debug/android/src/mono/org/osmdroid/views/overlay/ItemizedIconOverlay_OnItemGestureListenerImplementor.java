package mono.org.osmdroid.views.overlay;


public class ItemizedIconOverlay_OnItemGestureListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		org.osmdroid.views.overlay.ItemizedIconOverlay.OnItemGestureListener
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onItemLongPress:(ILjava/lang/Object;)Z:GetOnItemLongPress_ILjava_lang_Object_Handler:Osmdroid.Views.Overlay.ItemizedIconOverlay/IOnItemGestureListenerInvoker, OsmdroidAndroidBinding\n" +
			"n_onItemSingleTapUp:(ILjava/lang/Object;)Z:GetOnItemSingleTapUp_ILjava_lang_Object_Handler:Osmdroid.Views.Overlay.ItemizedIconOverlay/IOnItemGestureListenerInvoker, OsmdroidAndroidBinding\n" +
			"";
		mono.android.Runtime.register ("Osmdroid.Views.Overlay.ItemizedIconOverlay/IOnItemGestureListenerImplementor, OsmdroidAndroidBinding, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", ItemizedIconOverlay_OnItemGestureListenerImplementor.class, __md_methods);
	}


	public ItemizedIconOverlay_OnItemGestureListenerImplementor () throws java.lang.Throwable
	{
		super ();
		if (getClass () == ItemizedIconOverlay_OnItemGestureListenerImplementor.class)
			mono.android.TypeManager.Activate ("Osmdroid.Views.Overlay.ItemizedIconOverlay/IOnItemGestureListenerImplementor, OsmdroidAndroidBinding, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public boolean onItemLongPress (int p0, java.lang.Object p1)
	{
		return n_onItemLongPress (p0, p1);
	}

	private native boolean n_onItemLongPress (int p0, java.lang.Object p1);


	public boolean onItemSingleTapUp (int p0, java.lang.Object p1)
	{
		return n_onItemSingleTapUp (p0, p1);
	}

	private native boolean n_onItemSingleTapUp (int p0, java.lang.Object p1);

	java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
