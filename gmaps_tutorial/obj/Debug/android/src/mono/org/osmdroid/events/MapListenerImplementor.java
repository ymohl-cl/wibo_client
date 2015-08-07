package mono.org.osmdroid.events;


public class MapListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		org.osmdroid.events.MapListener
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onScroll:(Lorg/osmdroid/events/ScrollEvent;)Z:GetOnScroll_Lorg_osmdroid_events_ScrollEvent_Handler:Osmdroid.Events.IMapListenerInvoker, OsmdroidAndroidBinding\n" +
			"n_onZoom:(Lorg/osmdroid/events/ZoomEvent;)Z:GetOnZoom_Lorg_osmdroid_events_ZoomEvent_Handler:Osmdroid.Events.IMapListenerInvoker, OsmdroidAndroidBinding\n" +
			"";
		mono.android.Runtime.register ("Osmdroid.Events.IMapListenerImplementor, OsmdroidAndroidBinding, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", MapListenerImplementor.class, __md_methods);
	}


	public MapListenerImplementor () throws java.lang.Throwable
	{
		super ();
		if (getClass () == MapListenerImplementor.class)
			mono.android.TypeManager.Activate ("Osmdroid.Events.IMapListenerImplementor, OsmdroidAndroidBinding, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public boolean onScroll (org.osmdroid.events.ScrollEvent p0)
	{
		return n_onScroll (p0);
	}

	private native boolean n_onScroll (org.osmdroid.events.ScrollEvent p0);


	public boolean onZoom (org.osmdroid.events.ZoomEvent p0)
	{
		return n_onZoom (p0);
	}

	private native boolean n_onZoom (org.osmdroid.events.ZoomEvent p0);

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
