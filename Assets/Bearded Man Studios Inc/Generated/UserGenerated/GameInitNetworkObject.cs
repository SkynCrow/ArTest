using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedInterpol("{\"inter\":[0]")]
	public partial class GameInitNetworkObject : NetworkObject
	{
		public const int IDENTITY = 5;

		private byte[] _dirtyFields = new byte[1];

		#pragma warning disable 0067
		public event FieldChangedEvent fieldAltered;
		#pragma warning restore 0067
		[ForgeGeneratedField]
		private float _Time;
		public event FieldEvent<float> TimeChanged;
		public InterpolateFloat TimeInterpolation = new InterpolateFloat() { LerpT = 0f, Enabled = false };
		public float Time
		{
			get { return _Time; }
			set
			{
				// Don't do anything if the value is the same
				if (_Time == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x1;
				_Time = value;
				hasDirtyFields = true;
			}
		}

		public void SetTimeDirty()
		{
			_dirtyFields[0] |= 0x1;
			hasDirtyFields = true;
		}

		private void RunChange_Time(ulong timestep)
		{
			if (TimeChanged != null) TimeChanged(_Time, timestep);
			if (fieldAltered != null) fieldAltered("Time", _Time, timestep);
		}

		protected override void OwnershipChanged()
		{
			base.OwnershipChanged();
			SnapInterpolations();
		}
		
		public void SnapInterpolations()
		{
			TimeInterpolation.current = TimeInterpolation.target;
		}

		public override int UniqueIdentity { get { return IDENTITY; } }

		protected override BMSByte WritePayload(BMSByte data)
		{
			UnityObjectMapper.Instance.MapBytes(data, _Time);

			return data;
		}

		protected override void ReadPayload(BMSByte payload, ulong timestep)
		{
			_Time = UnityObjectMapper.Instance.Map<float>(payload);
			TimeInterpolation.current = _Time;
			TimeInterpolation.target = _Time;
			RunChange_Time(timestep);
		}

		protected override BMSByte SerializeDirtyFields()
		{
			dirtyFieldsData.Clear();
			dirtyFieldsData.Append(_dirtyFields);

			if ((0x1 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _Time);

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
				if (TimeInterpolation.Enabled)
				{
					TimeInterpolation.target = UnityObjectMapper.Instance.Map<float>(data);
					TimeInterpolation.Timestep = timestep;
				}
				else
				{
					_Time = UnityObjectMapper.Instance.Map<float>(data);
					RunChange_Time(timestep);
				}
			}
		}

		public override void InterpolateUpdate()
		{
			if (IsOwner)
				return;

			if (TimeInterpolation.Enabled && !TimeInterpolation.current.UnityNear(TimeInterpolation.target, 0.0015f))
			{
				_Time = (float)TimeInterpolation.Interpolate();
				//RunChange_Time(TimeInterpolation.Timestep);
			}
		}

		private void Initialize()
		{
			if (readDirtyFlags == null)
				readDirtyFlags = new byte[1];

		}

		public GameInitNetworkObject() : base() { Initialize(); }
		public GameInitNetworkObject(NetWorker networker, INetworkBehavior networkBehavior = null, int createCode = 0, byte[] metadata = null) : base(networker, networkBehavior, createCode, metadata) { Initialize(); }
		public GameInitNetworkObject(NetWorker networker, uint serverId, FrameStream frame) : base(networker, serverId, frame) { Initialize(); }

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}
