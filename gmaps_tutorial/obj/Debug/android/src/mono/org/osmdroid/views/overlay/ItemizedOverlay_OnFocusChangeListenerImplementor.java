package mono.org.osmdroid.views.overlay;


public class ItemizedOverlay_OnFocusChangeListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		org.osmdroid.views.overlay.ItemizedOverlay.OnFocusChangeListener
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onFocusChanged:(Lorg/osmdroid/views/overlay/ItemizedOverlay;Lorg/osmdroid/views/overlay/OverlayItem;)V:GetOnFocusChanged_Lorg_osmdroid_views_overlay_ItemizedOverlay_Lorg_osmdroid_views_overlay_OverlayItem_Handler:Osmdroid.Views.Overlay.ItemizedOverlay/IOnFocusChangeListenerInvoker, OsmdroidAndroidBinding\n" +
			"";
		mono.android.Runtime.register ("Osmdroid.Views.Overlay.ItemizedOverlay/IOnFocusChangeListenerImplementor, OsmdroidAndroidBinding, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", ItemizedOverlay_OnFocusChangeListenerImplementor.class, __md_methods);
	}


	public ItemizedOverlay_OnFocusChangeListenerImplementor () throws java.lang.Throwable
	{
		super ();
		if (getClass () == ItemizedOverlay_OnFocusChangeListenerImplementor.class)
			mono.android.TypeManager.Activate ("Osmdroid.Views.Overlay.ItemizedOverlay/IOnFocusChangeListenerImplementor, OsmdroidAndroidBinding, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onFocusChanged (org.osmdroid.views.overlay.ItemizedOverlay p0, org.osmdroid.views.overlay.OverlayItem p1)
	{
		n_onFocusChanged (p0, p1);
	}

	private native void n_onFocusChanged (org.osmdroid.views.overlay.ItemizedOverlay p0, org.osmdroid.views.overlay.OverlayItem p1);

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
