using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityTilemap = UnityEngine.Tilemaps.Tilemap;

namespace Tilemap {
	public class TileReskinner : MonoBehaviour {
		[SerializeField] private string _templateSpriteSheet;
		[SerializeField] private string _defaultTileSet;

		private readonly Dictionary<string, int> _tileNameToIndex = new();
		private UnityTilemap[]          _tilemaps;
		private string                  _currentTileSetName;

		[Button]
		private void DebugSetVector() {
			SetTileSet("vector");
		}
		[Button]
		private void DebugSetTemplate() {
			SetTileSet(_templateSpriteSheet);
		}

		private void Awake() {
			var template = Resources.LoadAll<Sprite>("TileSets/" + _templateSpriteSheet);
			for (var i = 0; i < template.Length; i++) {
				_tileNameToIndex.Add(template[i].name, i);
			}
		}

		void Start() {
			_tilemaps = GetComponentsInChildren<UnityTilemap>();
			if (_defaultTileSet != null)
				SetTileSet(_defaultTileSet);
		}

		public void SetTileSet(string tileSetName) {
			if (tileSetName == _currentTileSetName)
				return;

			_currentTileSetName = tileSetName;

			var tileSprites = Resources.LoadAll<Sprite>("TileSets/" + tileSetName);
			if (tileSprites.Length <= 0)
				return;

			foreach (UnityTilemap tilemap in _tilemaps) {
				for (int x = (int) tilemap.localBounds.min.x; x < tilemap.localBounds.max.x; x++) {
					for (int y = (int) tilemap.localBounds.min.y; y < tilemap.localBounds.max.y; y++) {
						var orgTile = tilemap.GetTile(new Vector3Int(x, y, 0)) as Tile;
						if (orgTile == null || orgTile.name.Length < 1)
							continue;

						if (_tileNameToIndex.TryGetValue(orgTile.name, out var index) && index<tileSprites.Length) {
							orgTile.sprite = tileSprites[index];
						}
					}
				}
				
				tilemap.RefreshAllTiles();
			}
		}
	}
}
