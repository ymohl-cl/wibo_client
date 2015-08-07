package mono.org.osmdroid.api;


public class OnCameraChangeListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		org.osmdroid.api.OnCameraChangeListener
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onCameraChange:(Lorg/osmdroid/api/IPosition;)V:GetOnCameraChange_Lorg_osmdroid_api_IPosition_Handler:Osmdroid.Api.IOnCameraChangeListenerInvoker, OsmdroidAndroidBinding\n" +
			"";
		mono.android.Runtime.register ("Osmdroid.Api.IOnCameraChangeListenerImplementor, OsmdroidAndroidBinding, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", OnCameraChangeListenerImplementor.class, __md_methods);
	}


	public OnCameraChangeListenerImplementor () throws java.lang.Throwable
	{
		super ();
		if (getClass () == OnCameraChangeListenerImplementor.class)
			mono.android.TypeManager.Activate ("Osmdroid.Api.IOnCameraChangeListenerImplementor, OsmdroidAndroidBinding, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCameraChange (org.osmdroid.api.IPosition p0)
	{
		n_onCameraChange (p0);
	}

	private native void n_onCameraChange (org.osmdroid.api.IPosition p0);

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
