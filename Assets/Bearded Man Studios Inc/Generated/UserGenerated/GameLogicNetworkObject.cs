using BeardedManStudios.Forge.Networking.Frame;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedInterpol("{\"inter\":[0,0]")]
	public partial class GameLogicNetworkObject : NetworkObject
	{
		public const int IDENTITY = 7;

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
		[ForgeGeneratedField]
		private bool _Wait;
		public event FieldEvent<bool> WaitChanged;
		public Interpolated<bool> WaitInterpolation = new Interpolated<bool>() { LerpT = 0f, Enabled = false };
		public bool Wait
		{
			get { return _Wait; }
			set
			{
				// Don't do anything if the value is the same
				if (_Wait == value)
					return;

				// Mark the field as dirty for the network to transmit
				_dirtyFields[0] |= 0x2;
				_Wait = value;
				hasDirtyFields = true;
			}
		}

		public void SetWaitDirty()
		{
			_dirtyFields[0] |= 0x2;
			hasDirtyFields = true;
		}

		private void RunChange_Wait(ulong timestep)
		{
			if (WaitChanged != null) WaitChanged(_Wait, timestep);
			if (fieldAltered != null) fieldAltered("Wait", _Wait, timestep);
		}

		protected override void OwnershipChanged()
		{
			base.OwnershipChanged();
			SnapInterpolations();
		}
		
		public void SnapInterpolations()
		{
			TimeInterpolation.current = TimeInterpolation.target;
			WaitInterpolation.current = WaitInterpolation.target;
		}

		public override int UniqueIdentity { get { return IDENTITY; } }

		protected override BMSByte WritePayload(BMSByte data)
		{
			UnityObjectMapper.Instance.MapBytes(data, _Time);
			UnityObjectMapper.Instance.MapBytes(data, _Wait);

			return data;
		}

		protected override void ReadPayload(BMSByte payload, ulong timestep)
		{
			_Time = UnityObjectMapper.Instance.Map<float>(payload);
			TimeInterpolation.current = _Time;
			TimeInterpolation.target = _Time;
			RunChange_Time(timestep);
			_Wait = UnityObjectMapper.Instance.Map<bool>(payload);
			WaitInterpolation.current = _Wait;
			WaitInterpolation.target = _Wait;
			RunChange_Wait(timestep);
		}

		protected override BMSByte SerializeDirtyFields()
		{
			dirtyFieldsData.Clear();
			dirtyFieldsData.Append(_dirtyFields);

			if ((0x1 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _Time);
			if ((0x2 & _dirtyFields[0]) != 0)
				UnityObjectMapper.Instance.MapBytes(dirtyFieldsData, _Wait);

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
			if ((0x2 & readDirtyFlags[0]) != 0)
			{
				if (WaitInterpolation.Enabled)
				{
					WaitInterpolation.target = UnityObjectMapper.Instance.Map<bool>(data);
					WaitInterpolation.Timestep = timestep;
				}
				else
				{
					_Wait = UnityObjectMapper.Instance.Map<bool>(data);
					RunChange_Wait(timestep);
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
			if (WaitInterpolation.Enabled && !WaitInterpolation.current.UnityNear(WaitInterpolation.target, 0.0015f))
			{
				_Wait = (bool)WaitInterpolation.Interpolate();
				//RunChange_Wait(WaitInterpolation.Timestep);
			}
		}

		private void Initialize()
		{
			if (readDirtyFlags == null)
				readDirtyFlags = new byte[1];

		}

		public GameLogicNetworkObject() : base() { Initialize(); }
		public GameLogicNetworkObject(NetWorker networker, INetworkBehavior networkBehavior = null, int createCode = 0, byte[] metadata = null) : base(networker, networkBehavior, createCode, metadata) { Initialize(); }
		public GameLogicNetworkObject(NetWorker networker, uint serverId, FrameStream frame) : base(networker, serverId, frame) { Initialize(); }

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}
