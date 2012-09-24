﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Simple.SAMS.Competitions.Data
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="SimpleITASAMS")]
	public partial class CompetitionsDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertCompetitionPlayer(CompetitionPlayer instance);
    partial void UpdateCompetitionPlayer(CompetitionPlayer instance);
    partial void DeleteCompetitionPlayer(CompetitionPlayer instance);
    partial void InsertCompetitionType(CompetitionType instance);
    partial void UpdateCompetitionType(CompetitionType instance);
    partial void DeleteCompetitionType(CompetitionType instance);
    partial void InsertPlayer(Player instance);
    partial void UpdatePlayer(Player instance);
    partial void DeletePlayer(Player instance);
    partial void InsertCompetition(Competition instance);
    partial void UpdateCompetition(Competition instance);
    partial void DeleteCompetition(Competition instance);
    #endregion
		
		public CompetitionsDataContext() : 
				base(global::Simple.SAMS.Competitions.Data.Properties.Settings.Default.SimpleITASAMSConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public CompetitionsDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public CompetitionsDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public CompetitionsDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public CompetitionsDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<CompetitionPlayer> CompetitionPlayers
		{
			get
			{
				return this.GetTable<CompetitionPlayer>();
			}
		}
		
		public System.Data.Linq.Table<CompetitionType> CompetitionTypes
		{
			get
			{
				return this.GetTable<CompetitionType>();
			}
		}
		
		public System.Data.Linq.Table<Player> Players
		{
			get
			{
				return this.GetTable<Player>();
			}
		}
		
		public System.Data.Linq.Table<Competition> Competitions
		{
			get
			{
				return this.GetTable<Competition>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.CompetitionPlayer")]
	public partial class CompetitionPlayer : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _CompetitionId;
		
		private int _PlayerId;
		
		private System.Nullable<int> _Rank;
		
		private EntityRef<Player> _Player;
		
		private EntityRef<Competition> _Competition;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnCompetitionIdChanging(int value);
    partial void OnCompetitionIdChanged();
    partial void OnPlayerIdChanging(int value);
    partial void OnPlayerIdChanged();
    partial void OnRankChanging(System.Nullable<int> value);
    partial void OnRankChanged();
    #endregion
		
		public CompetitionPlayer()
		{
			this._Player = default(EntityRef<Player>);
			this._Competition = default(EntityRef<Competition>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CompetitionId", DbType="Int NOT NULL", IsPrimaryKey=true)]
		public int CompetitionId
		{
			get
			{
				return this._CompetitionId;
			}
			set
			{
				if ((this._CompetitionId != value))
				{
					if (this._Competition.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnCompetitionIdChanging(value);
					this.SendPropertyChanging();
					this._CompetitionId = value;
					this.SendPropertyChanged("CompetitionId");
					this.OnCompetitionIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PlayerId", DbType="Int NOT NULL", IsPrimaryKey=true)]
		public int PlayerId
		{
			get
			{
				return this._PlayerId;
			}
			set
			{
				if ((this._PlayerId != value))
				{
					if (this._Player.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnPlayerIdChanging(value);
					this.SendPropertyChanging();
					this._PlayerId = value;
					this.SendPropertyChanged("PlayerId");
					this.OnPlayerIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Rank", DbType="Int")]
		public System.Nullable<int> Rank
		{
			get
			{
				return this._Rank;
			}
			set
			{
				if ((this._Rank != value))
				{
					this.OnRankChanging(value);
					this.SendPropertyChanging();
					this._Rank = value;
					this.SendPropertyChanged("Rank");
					this.OnRankChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Player_CompetitionPlayer", Storage="_Player", ThisKey="PlayerId", OtherKey="Id", IsForeignKey=true)]
		public Player Player
		{
			get
			{
				return this._Player.Entity;
			}
			set
			{
				Player previousValue = this._Player.Entity;
				if (((previousValue != value) 
							|| (this._Player.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._Player.Entity = null;
						previousValue.CompetitionPlayers.Remove(this);
					}
					this._Player.Entity = value;
					if ((value != null))
					{
						value.CompetitionPlayers.Add(this);
						this._PlayerId = value.Id;
					}
					else
					{
						this._PlayerId = default(int);
					}
					this.SendPropertyChanged("Player");
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Competition_CompetitionPlayer", Storage="_Competition", ThisKey="CompetitionId", OtherKey="Id", IsForeignKey=true)]
		public Competition Competition
		{
			get
			{
				return this._Competition.Entity;
			}
			set
			{
				Competition previousValue = this._Competition.Entity;
				if (((previousValue != value) 
							|| (this._Competition.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._Competition.Entity = null;
						previousValue.CompetitionPlayers.Remove(this);
					}
					this._Competition.Entity = value;
					if ((value != null))
					{
						value.CompetitionPlayers.Add(this);
						this._CompetitionId = value.Id;
					}
					else
					{
						this._CompetitionId = default(int);
					}
					this.SendPropertyChanged("Competition");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.CompetitionType")]
	public partial class CompetitionType : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private string _Name;
		
		private int _RowStatus;
		
		private System.DateTime _Created;
		
		private System.DateTime _Updated;
		
		private EntitySet<Competition> _Competitions;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnNameChanging(string value);
    partial void OnNameChanged();
    partial void OnRowStatusChanging(int value);
    partial void OnRowStatusChanged();
    partial void OnCreatedChanging(System.DateTime value);
    partial void OnCreatedChanged();
    partial void OnUpdatedChanging(System.DateTime value);
    partial void OnUpdatedChanged();
    #endregion
		
		public CompetitionType()
		{
			this._Competitions = new EntitySet<Competition>(new Action<Competition>(this.attach_Competitions), new Action<Competition>(this.detach_Competitions));
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Name", DbType="NVarChar(100) NOT NULL", CanBeNull=false)]
		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				if ((this._Name != value))
				{
					this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_RowStatus", DbType="Int NOT NULL")]
		public int RowStatus
		{
			get
			{
				return this._RowStatus;
			}
			set
			{
				if ((this._RowStatus != value))
				{
					this.OnRowStatusChanging(value);
					this.SendPropertyChanging();
					this._RowStatus = value;
					this.SendPropertyChanged("RowStatus");
					this.OnRowStatusChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Created", DbType="DateTime NOT NULL")]
		public System.DateTime Created
		{
			get
			{
				return this._Created;
			}
			set
			{
				if ((this._Created != value))
				{
					this.OnCreatedChanging(value);
					this.SendPropertyChanging();
					this._Created = value;
					this.SendPropertyChanged("Created");
					this.OnCreatedChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Updated", DbType="DateTime NOT NULL")]
		public System.DateTime Updated
		{
			get
			{
				return this._Updated;
			}
			set
			{
				if ((this._Updated != value))
				{
					this.OnUpdatedChanging(value);
					this.SendPropertyChanging();
					this._Updated = value;
					this.SendPropertyChanged("Updated");
					this.OnUpdatedChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="CompetitionType_Competition", Storage="_Competitions", ThisKey="Id", OtherKey="TypeId")]
		public EntitySet<Competition> Competitions
		{
			get
			{
				return this._Competitions;
			}
			set
			{
				this._Competitions.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_Competitions(Competition entity)
		{
			this.SendPropertyChanging();
			entity.CompetitionType = this;
		}
		
		private void detach_Competitions(Competition entity)
		{
			this.SendPropertyChanging();
			entity.CompetitionType = null;
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Player")]
	public partial class Player : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private string _Name;
		
		private string _IdNumber;
		
		private System.Nullable<int> _Rank;
		
		private int _RowStatus;
		
		private System.DateTime _Created;
		
		private System.DateTime _Updated;
		
		private EntitySet<CompetitionPlayer> _CompetitionPlayers;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnNameChanging(string value);
    partial void OnNameChanged();
    partial void OnIdNumberChanging(string value);
    partial void OnIdNumberChanged();
    partial void OnRankChanging(System.Nullable<int> value);
    partial void OnRankChanged();
    partial void OnRowStatusChanging(int value);
    partial void OnRowStatusChanged();
    partial void OnCreatedChanging(System.DateTime value);
    partial void OnCreatedChanged();
    partial void OnUpdatedChanging(System.DateTime value);
    partial void OnUpdatedChanged();
    #endregion
		
		public Player()
		{
			this._CompetitionPlayers = new EntitySet<CompetitionPlayer>(new Action<CompetitionPlayer>(this.attach_CompetitionPlayers), new Action<CompetitionPlayer>(this.detach_CompetitionPlayers));
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Name", DbType="NVarChar(100) NOT NULL", CanBeNull=false)]
		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				if ((this._Name != value))
				{
					this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_IdNumber", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string IdNumber
		{
			get
			{
				return this._IdNumber;
			}
			set
			{
				if ((this._IdNumber != value))
				{
					this.OnIdNumberChanging(value);
					this.SendPropertyChanging();
					this._IdNumber = value;
					this.SendPropertyChanged("IdNumber");
					this.OnIdNumberChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Rank", DbType="Int")]
		public System.Nullable<int> Rank
		{
			get
			{
				return this._Rank;
			}
			set
			{
				if ((this._Rank != value))
				{
					this.OnRankChanging(value);
					this.SendPropertyChanging();
					this._Rank = value;
					this.SendPropertyChanged("Rank");
					this.OnRankChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_RowStatus", DbType="Int NOT NULL")]
		public int RowStatus
		{
			get
			{
				return this._RowStatus;
			}
			set
			{
				if ((this._RowStatus != value))
				{
					this.OnRowStatusChanging(value);
					this.SendPropertyChanging();
					this._RowStatus = value;
					this.SendPropertyChanged("RowStatus");
					this.OnRowStatusChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Created", DbType="DateTime NOT NULL")]
		public System.DateTime Created
		{
			get
			{
				return this._Created;
			}
			set
			{
				if ((this._Created != value))
				{
					this.OnCreatedChanging(value);
					this.SendPropertyChanging();
					this._Created = value;
					this.SendPropertyChanged("Created");
					this.OnCreatedChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Updated", DbType="DateTime NOT NULL")]
		public System.DateTime Updated
		{
			get
			{
				return this._Updated;
			}
			set
			{
				if ((this._Updated != value))
				{
					this.OnUpdatedChanging(value);
					this.SendPropertyChanging();
					this._Updated = value;
					this.SendPropertyChanged("Updated");
					this.OnUpdatedChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Player_CompetitionPlayer", Storage="_CompetitionPlayers", ThisKey="Id", OtherKey="PlayerId")]
		public EntitySet<CompetitionPlayer> CompetitionPlayers
		{
			get
			{
				return this._CompetitionPlayers;
			}
			set
			{
				this._CompetitionPlayers.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_CompetitionPlayers(CompetitionPlayer entity)
		{
			this.SendPropertyChanging();
			entity.Player = this;
		}
		
		private void detach_CompetitionPlayers(CompetitionPlayer entity)
		{
			this.SendPropertyChanging();
			entity.Player = null;
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.Competition")]
	public partial class Competition : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private string _Name;
		
		private System.DateTime _StartDate;
		
		private int _TypeId;
		
		private int _RowStatus;
		
		private System.DateTime _Created;
		
		private System.DateTime _Updated;
		
		private string _ReferenceId;
		
		private EntitySet<CompetitionPlayer> _CompetitionPlayers;
		
		private EntityRef<CompetitionType> _CompetitionType;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnNameChanging(string value);
    partial void OnNameChanged();
    partial void OnStartDateChanging(System.DateTime value);
    partial void OnStartDateChanged();
    partial void OnTypeIdChanging(int value);
    partial void OnTypeIdChanged();
    partial void OnRowStatusChanging(int value);
    partial void OnRowStatusChanged();
    partial void OnCreatedChanging(System.DateTime value);
    partial void OnCreatedChanged();
    partial void OnUpdatedChanging(System.DateTime value);
    partial void OnUpdatedChanged();
    partial void OnReferenceIdChanging(string value);
    partial void OnReferenceIdChanged();
    #endregion
		
		public Competition()
		{
			this._CompetitionPlayers = new EntitySet<CompetitionPlayer>(new Action<CompetitionPlayer>(this.attach_CompetitionPlayers), new Action<CompetitionPlayer>(this.detach_CompetitionPlayers));
			this._CompetitionType = default(EntityRef<CompetitionType>);
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Name", DbType="NVarChar(100) NOT NULL", CanBeNull=false)]
		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				if ((this._Name != value))
				{
					this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_StartDate", DbType="DateTime NOT NULL")]
		public System.DateTime StartDate
		{
			get
			{
				return this._StartDate;
			}
			set
			{
				if ((this._StartDate != value))
				{
					this.OnStartDateChanging(value);
					this.SendPropertyChanging();
					this._StartDate = value;
					this.SendPropertyChanged("StartDate");
					this.OnStartDateChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_TypeId", DbType="Int NOT NULL")]
		public int TypeId
		{
			get
			{
				return this._TypeId;
			}
			set
			{
				if ((this._TypeId != value))
				{
					if (this._CompetitionType.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnTypeIdChanging(value);
					this.SendPropertyChanging();
					this._TypeId = value;
					this.SendPropertyChanged("TypeId");
					this.OnTypeIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_RowStatus", DbType="Int NOT NULL")]
		public int RowStatus
		{
			get
			{
				return this._RowStatus;
			}
			set
			{
				if ((this._RowStatus != value))
				{
					this.OnRowStatusChanging(value);
					this.SendPropertyChanging();
					this._RowStatus = value;
					this.SendPropertyChanged("RowStatus");
					this.OnRowStatusChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Created", DbType="DateTime NOT NULL")]
		public System.DateTime Created
		{
			get
			{
				return this._Created;
			}
			set
			{
				if ((this._Created != value))
				{
					this.OnCreatedChanging(value);
					this.SendPropertyChanging();
					this._Created = value;
					this.SendPropertyChanged("Created");
					this.OnCreatedChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Updated", DbType="DateTime NOT NULL")]
		public System.DateTime Updated
		{
			get
			{
				return this._Updated;
			}
			set
			{
				if ((this._Updated != value))
				{
					this.OnUpdatedChanging(value);
					this.SendPropertyChanging();
					this._Updated = value;
					this.SendPropertyChanged("Updated");
					this.OnUpdatedChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ReferenceId", DbType="NVarChar(100) NOT NULL", CanBeNull=false)]
		public string ReferenceId
		{
			get
			{
				return this._ReferenceId;
			}
			set
			{
				if ((this._ReferenceId != value))
				{
					this.OnReferenceIdChanging(value);
					this.SendPropertyChanging();
					this._ReferenceId = value;
					this.SendPropertyChanged("ReferenceId");
					this.OnReferenceIdChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="Competition_CompetitionPlayer", Storage="_CompetitionPlayers", ThisKey="Id", OtherKey="CompetitionId")]
		public EntitySet<CompetitionPlayer> CompetitionPlayers
		{
			get
			{
				return this._CompetitionPlayers;
			}
			set
			{
				this._CompetitionPlayers.Assign(value);
			}
		}
		
		[global::System.Data.Linq.Mapping.AssociationAttribute(Name="CompetitionType_Competition", Storage="_CompetitionType", ThisKey="TypeId", OtherKey="Id", IsForeignKey=true)]
		public CompetitionType CompetitionType
		{
			get
			{
				return this._CompetitionType.Entity;
			}
			set
			{
				CompetitionType previousValue = this._CompetitionType.Entity;
				if (((previousValue != value) 
							|| (this._CompetitionType.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._CompetitionType.Entity = null;
						previousValue.Competitions.Remove(this);
					}
					this._CompetitionType.Entity = value;
					if ((value != null))
					{
						value.Competitions.Add(this);
						this._TypeId = value.Id;
					}
					else
					{
						this._TypeId = default(int);
					}
					this.SendPropertyChanged("CompetitionType");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_CompetitionPlayers(CompetitionPlayer entity)
		{
			this.SendPropertyChanging();
			entity.Competition = this;
		}
		
		private void detach_CompetitionPlayers(CompetitionPlayer entity)
		{
			this.SendPropertyChanging();
			entity.Competition = null;
		}
	}
}
#pragma warning restore 1591