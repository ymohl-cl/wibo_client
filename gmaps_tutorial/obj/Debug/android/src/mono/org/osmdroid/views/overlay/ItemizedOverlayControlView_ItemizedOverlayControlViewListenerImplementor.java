package mono.org.osmdroid.views.overlay;


public class ItemizedOverlayControlView_ItemizedOverlayControlViewListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		org.osmdroid.views.overlay.ItemizedOverlayControlView.ItemizedOverlayControlViewListener
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onCenter:()V:GetOnCenterHandler:Osmdroid.Views.Overlay.ItemizedOverlayControlView/IItemizedOverlayControlViewListenerInvoker, OsmdroidAndroidBinding\n" +
			"n_onNavTo:()V:GetOnNavToHandler:Osmdroid.Views.Overlay.ItemizedOverlayControlView/IItemizedOverlayControlViewListenerInvoker, OsmdroidAndroidBinding\n" +
			"n_onNext:()V:GetOnNextHandler:Osmdroid.Views.Overlay.ItemizedOverlayControlView/IItemizedOverlayControlViewListenerInvoker, OsmdroidAndroidBinding\n" +
			"n_onPrevious:()V:GetOnPreviousHandler:Osmdroid.Views.Overlay.ItemizedOverlayControlView/IItemizedOverlayControlViewListenerInvoker, OsmdroidAndroidBinding\n" +
			"";
		mono.android.Runtime.register ("Osmdroid.Views.Overlay.ItemizedOverlayControlView/IItemizedOverlayControlViewListenerImplementor, OsmdroidAndroidBinding, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", ItemizedOverlayControlView_ItemizedOverlayControlViewListenerImplementor.class, __md_methods);
	}


	public ItemizedOverlayControlView_ItemizedOverlayControlViewListenerImplementor () throws java.lang.Throwable
	{
		super ();
		if (getClass () == ItemizedOverlayControlView_ItemizedOverlayControlViewListenerImplementor.class)
			mono.android.TypeManager.Activate ("Osmdroid.Views.Overlay.ItemizedOverlayControlView/IItemizedOverlayControlViewListenerImplementor, OsmdroidAndroidBinding, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCenter ()
	{
		n_onCenter ();
	}

	private native void n_onCenter ();


	public void onNavTo ()
	{
		n_onNavTo ();
	}

	private native void n_onNavTo ();


	public void onNext ()
	{
		n_onNext ();
	}

	private native void n_onNext ();


	public void onPrevious ()
	{
		n_onPrevious ();
	}

	private native void n_onPrevious ();

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
