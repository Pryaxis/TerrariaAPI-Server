﻿using System;
{
	{
		{
			return NPCUtils.SearchForTarget(null, position, flags, playerFilter, npcFilter);
		}

		public static NPCUtils.TargetSearchResults SearchForTarget(NPC searcher, NPCUtils.TargetSearchFlag flags = NPCUtils.TargetSearchFlag.All, NPCUtils.SearchFilter<Player> playerFilter = null, NPCUtils.SearchFilter<NPC> npcFilter = null)
		{
			return NPCUtils.SearchForTarget(searcher, searcher.Center, flags, playerFilter, npcFilter);
		}

		public static NPCUtils.TargetSearchResults SearchForTarget(NPC searcher, Vector2 position, NPCUtils.TargetSearchFlag flags = NPCUtils.TargetSearchFlag.All, NPCUtils.SearchFilter<Player> playerFilter = null, NPCUtils.SearchFilter<NPC> npcFilter = null)
		{

			float num = 3.40282347E+38f;
			int nearestNPCIndex = -1;
			float num2 = 3.40282347E+38f;
			float nearestTankDistance = 3.40282347E+38f;
			int nearestTankIndex = -1;
			NPCUtils.TargetType tankType = NPCUtils.TargetType.Player;

			if (flags.HasFlag(NPCUtils.TargetSearchFlag.NPCs))
			{
				for (int i = 0; i < 200; i++)
				{
					NPC nPC = Main.npc[i];
					if (nPC.active && (npcFilter == null || npcFilter(nPC)))
					{
						float num3 = Vector2.DistanceSquared(position, nPC.Center);
						if (num3 < num)
						{
							nearestNPCIndex = i;
							num = num3;
						}
					}
				}
			}

			if (flags.HasFlag(NPCUtils.TargetSearchFlag.Players))
			{
				for (int j = 0; j < 255; j++)
				{
					Player player = Main.player[j];

					if (player.active && !player.dead && !player.ghost && (playerFilter == null || playerFilter(player)))
					{
						float num4 = Vector2.Distance(position, player.Center);
						float num5 = num4 - (float)player.aggro;
						bool flag = searcher != null && player.npcTypeNoAggro[searcher.type];
						if (searcher != null && flag && searcher.direction == 0)
						{
							num5 += 1000f;
						}
						if (num5 < num2)
						{
							nearestTankIndex = j;
							num2 = num5;
							nearestTankDistance = num4;
							tankType = NPCUtils.TargetType.Player;
						}
						if (player.tankPet >= 0 && !flag)
						{
							Vector2 center = Main.projectile[player.tankPet].Center;
							num4 = Vector2.Distance(position, center);
							num5 = num4 - 200f;

							if (num5 < num2 && num5 < 200f && Collision.CanHit(position, 0, 0, center, 0, 0))
							{
								nearestTankIndex = j;
								num2 = num5;
								nearestTankDistance = num4;
								tankType = NPCUtils.TargetType.TankPet;
							}
						}
					}
				}
			}
			return new NPCUtils.TargetSearchResults(searcher, nearestNPCIndex, (float)Math.Sqrt((double)num), nearestTankIndex, nearestTankDistance, num2, tankType);
		}

		public static void TargetClosestBetsy(NPC searcher, bool faceTarget = true, Vector2? checkPosition = null)
		{
			NPCUtils.TargetSearchResults targetSearchResults = NPCUtils.SearchForTarget(searcher, NPCUtils.TargetSearchFlag.All, null, new NPCUtils.SearchFilter<NPC>(NPCUtils.SearchFilters.OnlyCrystal));
			if (!targetSearchResults.FoundTarget)
			{
				return;
			}
			NPCUtils.TargetType value = targetSearchResults.NearestTargetType;
			if (targetSearchResults.FoundTank && !targetSearchResults.NearestTankOwner.dead)
			{
				value = NPCUtils.TargetType.Player;
			}
			searcher.target = targetSearchResults.NearestTargetIndex;
			searcher.targetRect = targetSearchResults.NearestTargetHitbox;
			if (searcher.ShouldFaceTarget(ref targetSearchResults, new NPCUtils.TargetType?(value)) && faceTarget)
			{
				searcher.FaceTarget();
			}
		}

		public static void TargetClosestOldOnesInvasion(NPC searcher, bool faceTarget = true, Vector2? checkPosition = null)
		{
			NPCUtils.SearchFilters.v2_1 = searcher.Center;
			NPCUtils.SearchFilters.f_1 = 200f;
			NPCUtils.TargetSearchResults targetSearchResults = NPCUtils.SearchForTarget(searcher, NPCUtils.TargetSearchFlag.All, new NPCUtils.SearchFilter<Player>(NPCUtils.SearchFilters.OnlyPlayersInCertainDistance), new NPCUtils.SearchFilter<NPC>(NPCUtils.SearchFilters.OnlyCrystal));
			if (!targetSearchResults.FoundTarget)
			{
				return;
			}
			searcher.target = targetSearchResults.NearestTargetIndex;
			searcher.targetRect = targetSearchResults.NearestTargetHitbox;
			if (searcher.ShouldFaceTarget(ref targetSearchResults, null) && faceTarget)
			{
				searcher.FaceTarget();
			}
		}

		public static class SearchFilters
		{
			{
				return npc.type == 548 && !npc.dontTakeDamageFromHostiles;
			}
			public static bool OnlyPlayersInCertainDistance(Player player)
			{
				return player.active && player.Distance(NPCUtils.SearchFilters.v2_1) <= NPCUtils.SearchFilters.f_1;
			}
			public static float f_1;
			public static Vector2 v2_1;
		}

		[Flags]
		public enum TargetSearchFlag
		{
		}

		public struct TargetSearchResults
		{
			{
				this._nearestNPCIndex = nearestNPCIndex;
				this._nearestNPCDistance = nearestNPCDistance;
				this._nearestTankIndex = nearestTankIndex;
				this._adjustedTankDistance = adjustedTankDistance;
				this._nearestTankDistance = nearestTankDistance;
				this._nearestTankType = tankType;
				if (this._nearestNPCIndex != -1 && this._nearestTankIndex != -1)
				{
					if (this._nearestNPCDistance < this._adjustedTankDistance)
					{
						this._nearestTargetType = NPCUtils.TargetType.NPC;
						return;
					}
					this._nearestTargetType = tankType;
					return;
				}
				else
				{
					if (this._nearestNPCIndex != -1)
					{
						this._nearestTargetType = NPCUtils.TargetType.NPC;
						return;
					}
					if (this._nearestTankIndex != -1)
					{
						this._nearestTargetType = tankType;
						return;
					}
					this._nearestTargetType = NPCUtils.TargetType.None;
					return;
				}
			}

			public float AdjustedTankDistance
			{
				{
					return this._adjustedTankDistance;
				}
			}

			public bool FoundNPC
			{
				{
					return this._nearestNPCIndex != -1;
				}
			}

			public bool FoundTank
			{
				{
					return this._nearestTankIndex != -1;
				}
			}

			public bool FoundTarget
			{
				{
					return this._nearestTargetType != NPCUtils.TargetType.None;
				}
			}

			public NPC NearestNPC
			{
				{
					if (this._nearestNPCIndex != -1)
					{
						return Main.npc[this._nearestNPCIndex];
					}
					return null;
				}
			}

			public float NearestNPCDistance
			{
				{
					return this._nearestNPCDistance;
				}
			}

			public int NearestNPCIndex
			{
				{
					return this._nearestNPCIndex;
				}
			}

			public float NearestTankDistance
			{
				{
					return this._nearestTankDistance;
				}
			}

			public Player NearestTankOwner
			{
				{
					if (this._nearestTankIndex != -1)
					{
						return Main.player[this._nearestTankIndex];
					}
					return null;
				}
			}

			public int NearestTankOwnerIndex
			{
				{
					return this._nearestTankIndex;
				}
			}

			public NPCUtils.TargetType NearestTankType
			{
				{
					return this._nearestTankType;
				}
			}

			public Rectangle NearestTargetHitbox
			{
				{
					switch (this._nearestTargetType)
					{
						case NPCUtils.TargetType.NPC:
							return this.NearestNPC.Hitbox;
						case NPCUtils.TargetType.Player:
							return this.NearestTankOwner.Hitbox;
						case NPCUtils.TargetType.TankPet:
							return Main.projectile[this.NearestTankOwner.tankPet].Hitbox;
						default:
							return Rectangle.Empty;
					}
				}
			}

			public int NearestTargetIndex
			{
				{
					switch (this._nearestTargetType)
					{
						case NPCUtils.TargetType.NPC:
							return this.NearestNPC.WhoAmIToTargettingIndex;
						case NPCUtils.TargetType.Player:
						case NPCUtils.TargetType.TankPet:
							return this._nearestTankIndex;
						default:
							return -1;
					}
				}
			}

			public NPCUtils.TargetType NearestTargetType
			{
				{
					return this._nearestTargetType;
				}
			}

			private NPCUtils.TargetType _nearestTargetType;

			private int _nearestNPCIndex;

			private float _nearestNPCDistance;

			private int _nearestTankIndex;

			private float _nearestTankDistance;

			private float _adjustedTankDistance;

			private NPCUtils.TargetType _nearestTankType;
		}

		public enum TargetType
		{
		}
	}
}