using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedInterpol("{\"inter\":[0]")]
	public partial class GameLobbyNetworkObject : NetworkObject
	{
		public const int IDENTITY = 6;

		private byte[] _dirtyFields = new byte[1];

		#pragma warning disable 0067
		public event FieldChangedEvent fieldAltered;
		#pragma warning restore 0067
		[ForgeGeneratedField]
		private bool _Ready;
		public event FieldEvent<bool> ReadyChanged;
		public Interpolated<bool> ReadyInterpolation = new Interpolated<bool>() { LerpT = 0f, Enabled = false };
		public bool Ready
		{
			get { return _Ready; }
			set
			{
				// Don't do anything if the value is the same
				if (_Ready == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x1;
				_Ready = value;
				hasDirtyFields = true;
			}
		}

		public void SetReadyDirty()
		{
			_dirtyFields[0] |= 0x1;
			hasDirtyFields = true;
		}

		private void RunChange_Ready(ulong timestep)
		{
			if (ReadyChanged != null) ReadyChanged(_Ready, timestep);
			if (fieldAltered != null) fieldAltered("Ready", _Ready, timestep);
		}

		protected override void OwnershipChanged()
		{
			base.OwnershipChanged();
			SnapInterpolations();
		}
		
		public void SnapInterpolations()
		{
			ReadyInterpolation.current = ReadyInterpolation.target;
		}

		public override int UniqueIdentity { get { return IDENTITY; } }

		protected override BMSByte WritePayload(BMSByte data)
		{
			UnityObjectMapper.Instance.MapBytes(data, _Ready);

			return data;
		}

		protected override void ReadPayload(BMSByte payload, ulong timestep)
		{
			_Ready = UnityObjectMapper.Instance.Map<bool>(payload);
			ReadyInterpolation.current = _Ready;
			ReadyInterpolation.target = _Ready;
			RunChange_Ready(timestep);
		}

		protected override BMSByte SerializeDirtyFields()
		{
			dirtyFieldsData.Clear();
			dirtyFieldsData.Append(_dirtyFields);

			if ((0x1 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _Ready);

			// Reset all the dirty fields
			for (int i = 0; i < _dirtyFields.Length; i++)
				_dirtyFields[i] = 0;

			return dirtyFieldsData;
		}

		protected override void ReadDirtyFields(BMSByte data, ulong timestep)
		{
			if (readDirtyFlags == null)
				Initialize();

			Buffer.BlockCopy(data.byteArr, data.StartIndex(), readDirtyFlags, 0, readDirtyFlags.Length);
			data.MoveStartIndex(readDirtyFlags.Length);

			if ((0x1 & readDirtyFlags[0]) != 0)
			{
				if (ReadyInterpolation.Enabled)
				{
					ReadyInterpolation.target = UnityObjectMapper.Instance.Map<bool>(data);
					ReadyInterpolation.Timestep = timestep;
				}
				else
				{
					_Ready = UnityObjectMapper.Instance.Map<bool>(data);
					RunChange_Ready(timestep);
				}
			}
		}

		public override void InterpolateUpdate()
		{
			if (IsOwner)
				return;

			if (ReadyInterpolation.Enabled && !ReadyInterpolation.current.UnityNear(ReadyInterpolation.target, 0.0015f))
			{
				_Ready = (bool)ReadyInterpolation.Interpolate();
				//RunChange_Ready(ReadyInterpolation.Timestep);
			}
		}

		private void Initialize()
		{
			if (readDirtyFlags == null)
				readDirtyFlags = new byte[1];

		}

		public GameLobbyNetworkObject() : base() { Initialize(); }
		public GameLobbyNetworkObject(NetWorker networker, INetworkBehavior networkBehavior = null, int createCode = 0, byte[] metadata = null) : base(networker, networkBehavior, createCode, metadata) { Initialize(); }
		public GameLobbyNetworkObject(NetWorker networker, uint serverId, FrameStream frame) : base(networker, serverId, frame) { Initialize(); }

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}
