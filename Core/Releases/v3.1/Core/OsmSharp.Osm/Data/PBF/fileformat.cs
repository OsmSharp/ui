//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: fileformat.proto
namespace OsmSharp.Osm.Data.PBF
{
  /// <summary>
  /// 
  /// </summary>
  [global::ProtoBuf.ProtoContract(Name=@"Blob")]
  public partial class Blob : global::ProtoBuf.IExtensible
  {
    /// <summary>
    /// 
    /// </summary>
    public Blob() {}
    

    private byte[] _raw = null;
    /// <summary>
    /// 
    /// </summary>
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"raw", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public byte[] raw
    {
      get { return _raw; }
      set { _raw = value; }
    }

    private int _raw_size = default(int);
    /// <summary>
    /// 
    /// </summary>
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"raw_size", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int raw_size
    {
      get { return _raw_size; }
      set { _raw_size = value; }
    }

    private byte[] _zlib_data = null;
    /// <summary>
    /// 
    /// </summary>
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"zlib_data", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public byte[] zlib_data
    {
      get { return _zlib_data; }
      set { _zlib_data = value; }
    }

    private byte[] _lzma_data = null;
    /// <summary>
    /// 
    /// </summary>
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"lzma_data", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public byte[] lzma_data
    {
      get { return _lzma_data; }
      set { _lzma_data = value; }
    }

    private byte[] _bzip2_data = null;
    /// <summary>
    /// 
    /// </summary>
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"bzip2_data", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public byte[] bzip2_data
    {
      get { return _bzip2_data; }
      set { _bzip2_data = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  /// <summary>
  /// 
  /// </summary>
  [global::ProtoBuf.ProtoContract(Name=@"BlockHeader")]
  public partial class BlockHeader : global::ProtoBuf.IExtensible
  {
    /// <summary>
    /// 
    /// </summary>
    public BlockHeader() {}
    
    private string _type;
    /// <summary>
    /// 
    /// </summary>
    [global::ProtoBuf.ProtoMember(1, IsRequired = true, Name=@"type", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public string type
    {
      get { return _type; }
      set { _type = value; }
    }

    private byte[] _indexdata = null;
    /// <summary>
    /// 
    /// </summary>
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"indexdata", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public byte[] indexdata
    {
      get { return _indexdata; }
      set { _indexdata = value; }
    }
    private int _datasize;
    /// <summary>
    /// 
    /// </summary>
    [global::ProtoBuf.ProtoMember(3, IsRequired = true, Name=@"datasize", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    public int datasize
    {
      get { return _datasize; }
      set { _datasize = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}
