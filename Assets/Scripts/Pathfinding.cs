using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour {

	private List<Tile> openSet;
	private List<Tile> closedSet;

	public List<Tile> FindPath(Tile start, Tile end) {

		openSet = new List<Tile>();
		closedSet = new List<Tile>();
		
		List<Tile> path = new List<Tile>();

		start.SetCosts(start, end);
		openSet.Add(start);

		while (openSet.Count > 0) {
			Tile current = openSet[0];
			for (int i = 1; i < openSet.Count; i++) {
				if (openSet[i].FCost < current.FCost) {
					current = openSet[i];
				}
			}

			openSet.Remove(current);
			closedSet.Add(current);

			if (current == end) {
				return RetracePath(start, end);
			}

			foreach (Tile tile in current.connected) {
				if (closedSet.Contains(tile)) {
					continue;
				}

				// if new path to neighbor is shorter OR neighbor is not in openSet
				if (!openSet.Contains(tile)) {
					// set fCost of neighbor
					tile.SetCosts(start, end);
					// set parent of neighbor to current
					tile.Parent = current;
					// if neighbor is not in openSet
						// add neighbor to open
						openSet.Add(tile);

				}
			}
		}

		return path;
	}

	List<Tile> RetracePath(Tile start, Tile end) {
		List<Tile> path = new List<Tile>();
		Tile current = end;
		while (current != start) {
			path.Add(current);
			current = current.Parent;
		}
		path.Reverse();
		return path;
	}

}
