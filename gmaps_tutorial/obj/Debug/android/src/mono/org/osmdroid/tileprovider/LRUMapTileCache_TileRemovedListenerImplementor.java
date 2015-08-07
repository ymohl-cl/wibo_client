package mono.org.osmdroid.tileprovider;


public class LRUMapTileCache_TileRemovedListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		org.osmdroid.tileprovider.LRUMapTileCache.TileRemovedListener
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onTileRemoved:(Lorg/osmdroid/tileprovider/MapTile;)V:GetOnTileRemoved_Lorg_osmdroid_tileprovider_MapTile_Handler:Osmdroid.TileProvider.LRUMapTileCache/ITileRemovedListenerInvoker, OsmdroidAndroidBinding\n" +
			"";
		mono.android.Runtime.register ("Osmdroid.TileProvider.LRUMapTileCache/ITileRemovedListenerImplementor, OsmdroidAndroidBinding, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", LRUMapTileCache_TileRemovedListenerImplementor.class, __md_methods);
	}


	public LRUMapTileCache_TileRemovedListenerImplementor () throws java.lang.Throwable
	{
		super ();
		if (getClass () == LRUMapTileCache_TileRemovedListenerImplementor.class)
			mono.android.TypeManager.Activate ("Osmdroid.TileProvider.LRUMapTileCache/ITileRemovedListenerImplementor, OsmdroidAndroidBinding, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onTileRemoved (org.osmdroid.tileprovider.MapTile p0)
	{
		n_onTileRemoved (p0);
	}

	private native void n_onTileRemoved (org.osmdroid.tileprovider.MapTile p0);

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
