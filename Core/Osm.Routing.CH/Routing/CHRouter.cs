using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Osm.Data.Core.CH;
using Osm.Data.Core.CH.Primitives;
using Osm.Data.Core.Sparse.Primitives;
using Tools.Math.Geo;
using Tools.Math.Shapes;
using Tools.Math.Geo.Factory;

namespace Osm.Routing.CH.Routing
{
	/// <summary>
	/// A router for CH.
	/// </summary>
	public class CHRouter : ICHData
	{
		/// <summary>
		/// The CH data.
		/// </summary>
		private ICHData _data;

		/// <summary>
		/// Creates a new CH router on the givend data.
		/// </summary>
		/// <param name="data"></param>
		public CHRouter(ICHData data)
		{
			_data = data;

			_ch_vertices = new Dictionary<long, CHVertex>();
		}

		#region Bi-directional Many-to-Many

		/// <summary>
		/// Calculates all the weights between all the vertices.
		/// </summary>
		/// <param name="froms"></param>
		/// <param name="tos"></param>
		/// <returns></returns>
		public float[][] CalculateManyToManyWeights(CHVertex[] froms, CHVertex[] tos)
		{
			// TODO: implement switching of from/to when to < from.

			// keep a list of distances to the given vertices while performance backward search.
			Dictionary<long, Dictionary<long, float>> buckets = new Dictionary<long, Dictionary<long, float>>();
			foreach (CHVertex to in tos)
			{
				this.SearchBackwardIntoBucket(buckets, to);
			}

			// conduct a forward search from each source.
			float[][] weights = new float[froms.Length][];
			for (int idx = 0; idx < froms.Length; idx++)
			{
				CHVertex from = froms[idx];

				// calculate all from's.
				Dictionary<long, float> result =
					this.SearchForwardFromBucket(buckets, from, tos);

				float[] to_weights = new float[tos.Length];
				for (int to_idx = 0; to_idx < tos.Length; to_idx++)
				{
					to_weights[to_idx] = result[tos[to_idx].Id];
				}

				weights[idx] = to_weights;
				result.Clear();
			}
			return weights;
		}

		/// <summary>
		/// Searches backwards and puts the weigths from the to-vertex into the buckets list.
		/// </summary>
		/// <param name="queue"></param>
		/// <returns></returns>
		private void SearchBackwardIntoBucket(Dictionary<long, Dictionary<long, float>> buckets, CHVertex to)
		{
			Dictionary<long, CHPathSegment> settled_vertices =
				new Dictionary<long, CHPathSegment>();
			CHPriorityQueue queue = new CHPriorityQueue();
			queue.Push(new CHPathSegment(to.Id));

			// get the current vertex with the smallest weight.
			while (queue.Count > 0) // TODO: work on a stopping condition?
			{
				CHPathSegment current = queue.Pop();

				// add to the settled vertices.
				CHPathSegment previous_linked_route;
				if (settled_vertices.TryGetValue(current.VertexId, out previous_linked_route))
				{
					if (previous_linked_route.Weight > current.Weight)
					{
						// settle the vertex again if it has a better weight.
						settled_vertices[current.VertexId] = current;
					}
				}
				else
				{
					// settled the vertex.
					settled_vertices[current.VertexId] = current;
				}

				// add to bucket.
				Dictionary<long, float> bucket;
				if (!buckets.TryGetValue(current.VertexId, out bucket))
				{
					bucket = new Dictionary<long, float>();
					buckets.Add(current.VertexId, bucket);
				}
				bucket[to.Id] = (float) current.Weight;

				// get neighbours.
				CHVertex vertex = this.GetCHVertex(current.VertexId);

				// add the neighbours to the queue.
				foreach (CHVertexNeighbour neighbour in vertex.BackwardNeighbours)
				{
					if (!settled_vertices.ContainsKey(neighbour.Id))
					{
						// if not yet settled.
						CHPathSegment route_to_neighbour = new CHPathSegment(
							neighbour.Id, current.Weight + neighbour.Weight, current);
						queue.Push(route_to_neighbour);
					}
				}
			}
		}

		/// <summary>
		/// Searches forward and uses the bucket to calculate smallest weights.
		/// </summary>
		/// <param name="buckets"></param>
		/// <param name="from"></param>
		private Dictionary<long, float> SearchForwardFromBucket(Dictionary<long, Dictionary<long, float>> buckets,
		                                                        CHVertex from, CHVertex[] tos)
		{
			// intialize weights.
			Dictionary<long, float> results = new Dictionary<long, float>();
			//HashSet<long> permanent_results = new HashSet<long>();
			Dictionary<long, float> tentative_results = new Dictionary<long, float>();

			Dictionary<long, CHPathSegment> settled_vertices =
				new Dictionary<long, CHPathSegment>();
			CHPriorityQueue queue = new CHPriorityQueue();
			queue.Push(new CHPathSegment(from.Id));

			// get the current vertex with the smallest weight.
			int k = 0;
			while (queue.Count > 0) // TODO: work on a stopping condition?
			{
				CHPathSegment current = queue.Pop();
				k++;

				//// remove from the tentative results list.
				//if (k > 1)
				//{
				//    HashSet<long> to_remove_set = new HashSet<long>();
				//    foreach (KeyValuePair<long, float> result in tentative_results)
				//    {
				//        if (result.Value < current.Weight)
				//        {
				//            to_remove_set.Add(result.Key);
				//            if (!results.ContainsKey(result.Key))
				//            {
				//                results.Add(result.Key, result.Value);
				//            }
				//        }
				//    }
				//    foreach (long to_remove in to_remove_set)
				//    {
				//        tentative_results.Remove(to_remove);
				//    }
				//    k = 0;
				//}

				// stop search if all results found.
				if (results.Count == tos.Length)
				{
					break;
				}
				// add to the settled vertices.
				CHPathSegment previous_linked_route;
				if (settled_vertices.TryGetValue(current.VertexId, out previous_linked_route))
				{
					if (previous_linked_route.Weight > current.Weight)
					{
						// settle the vertex again if it has a better weight.
						settled_vertices[current.VertexId] = current;
					}
				}
				else
				{
					// settled the vertex.
					settled_vertices[current.VertexId] = current;
				}

				// search the bucket.
				Dictionary<long, float> bucket;
				if (buckets.TryGetValue(current.VertexId, out bucket))
				{
					// there is a bucket!
					foreach (KeyValuePair<long, float> bucket_entry in bucket)
					{
						//if (!permanent_results.Contains(bucket_entry.Key))
						//{
						float found_distance = bucket_entry.Value + (float) current.Weight;
						float tentative_distance;
						if (tentative_results.TryGetValue(bucket_entry.Key, out tentative_distance))
						{
							if (found_distance < tentative_distance)
							{
								tentative_results[bucket_entry.Key] = found_distance;
							}

							if (tentative_distance < current.Weight)
							{
								tentative_results.Remove(bucket_entry.Key);
								results.Add(bucket_entry.Key, tentative_distance);
							}
						}
						//else
						//{ // there was no result yet!
						//    tentative_results[bucket_entry.Key] = found_distance;
						//}
						//}
					}
				}

				// get neighbours.
				CHVertex vertex = this.GetCHVertex(current.VertexId);

				// add the neighbours to the queue.
				foreach (CHVertexNeighbour neighbour in vertex.ForwardNeighbours)
				{
					if (!settled_vertices.ContainsKey(neighbour.Id))
					{
						// if not yet settled.
						CHPathSegment route_to_neighbour = new CHPathSegment(
							neighbour.Id, current.Weight + neighbour.Weight, current);
						queue.Push(route_to_neighbour);
					}
				}
			}

			foreach (CHVertex to in tos)
			{
				if (!tentative_results.ContainsKey(to.Id))
				{
					if (results.ContainsKey(to.Id))
					{
						tentative_results[to.Id] = results[to.Id];
					}
					else
					{
						tentative_results[to.Id] = float.MaxValue;
					}
				}
			}

			return tentative_results;
		}

		#endregion

		#region Bi-directional Point-To-Point


		/// <summary>
		/// Calculates the actual route from from to to.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		public CHPathSegment Calculate(long from, long to)
		{
			// calculate the result.
			CHResult result = this.CalculateInternal(from, to, -1, double.MaxValue, int.MaxValue);

			// construct the route.
			CHPathSegment route = result.Forward;
			CHPathSegment next = result.Backward;
			while (next != null && next.From != null)
			{
				route = new CHPathSegment(next.From.VertexId,
				                          next.Weight + route.Weight, route);
				next = next.From;
			}

			// report a path segment.
			this.NotifyCHPathSegment(route);

			return this.ExpandPath(route);
		}

		/// <summary>
		/// Calculates the weight from from to to.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		public double CalculateWeight(long from, long to)
		{
			// calculate the result.
			CHResult result = this.CalculateInternal(from, to, -1, double.MaxValue, int.MaxValue);

			// construct the route.
			return result.Forward.Weight + result.Backward.Weight;
		}

		/// <summary>
		/// Calculates the weight from from to to.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		public double CalculateWeight(long from, long to, long exception)
		{
			return this.CalculateWeight(from, to, exception, double.MaxValue);
		}

		/// <summary>
		/// Calculates the weight from from to to.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		public double CalculateWeight(long from, long to, long exception, double max)
		{
			// calculate the result.
			CHResult result = this.CalculateInternal(from, to, exception, max, int.MaxValue);

			// construct the route.
			if (result.Forward != null && result.Backward != null)
			{
				return result.Forward.Weight + result.Backward.Weight;
			}
			return double.MaxValue;
		}

		/// <summary>
		/// Calculates the weight from from to to.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		public double CalculateWeight(long from, long to, long exception, double max, int max_settles)
		{
			// calculate the result.
			CHResult result = this.CalculateInternal(from, to, exception, max, max_settles);

			// construct the route.
			if (result.Forward != null && result.Backward != null)
			{
				return result.Forward.Weight + result.Backward.Weight;
			}
			return double.MaxValue;
		}

		/// <summary>
		/// Calculates a shortest path between the two given vertices.
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		private CHResult CalculateInternal(long from, long to, long exception, double max, int max_settles)
		{
			// keep settled vertices.
			//Dictionary<long, CHPathSegment> settled_backward_vertices = 
			//    new Dictionary<long, CHPathSegment>();
			//Dictionary<long, CHPathSegment> settled_forward_vertices = 
			//    new Dictionary<long, CHPathSegment>();
			CHQueue settled_vertices = new CHQueue();

			// initialize the queues.
			CHPriorityQueue queue_forward = new CHPriorityQueue();
			CHPriorityQueue queue_backward = new CHPriorityQueue();

			// add the from vertex to the forward queue.
			queue_forward.Push(new CHPathSegment(from));

			// add the from vertex to the backward queue.
			queue_backward.Push(new CHPathSegment(to));

			// keep looping until stopping conditions are met.
			CHBest best = this.CalculateBest(settled_vertices);

			// calculate stopping conditions.
			double queue_backward_weight = queue_backward.PeekWeight();
			double queue_forward_weight = queue_forward.PeekWeight();
			while (!(queue_backward.Count == 0 && queue_forward.Count == 0) &&
			       (best.Weight > queue_backward_weight && best.Weight > queue_forward_weight) &&
			       (max >= queue_backward_weight && max >= queue_forward_weight) &&
			       (max_settles >= (settled_vertices.Forward.Count + settled_vertices.Backward.Count)))
			{
				// keep looping until stopping conditions.

				// do a forward search.
				if (queue_forward.Count > 0)
				{
					this.SearchForward(settled_vertices, queue_forward, exception);
				}

				// do a backward search.
				if (queue_backward.Count > 0)
				{
					this.SearchBackward(settled_vertices, queue_backward, exception);
				}

				// calculate the new best if any.
				best = this.CalculateBest(settled_vertices);

				// calculate stopping conditions.
				if (queue_forward.Count > 0)
				{
					queue_forward_weight = queue_forward.PeekWeight();
				}
				if (queue_backward.Count > 0)
				{
					queue_backward_weight = queue_backward.PeekWeight();
				}
			}

			// return forward/backward routes.
			CHResult result = new CHResult();
			if (best.VertexId < 0)
			{
				// no route was found!

			}
			else
			{
				// construct the existing route.
				//result.Forward = settled_forward_vertices[best.VertexId];
				//result.Backward = settled_backward_vertices[best.VertexId];
				result.Forward = settled_vertices.Forward[best.VertexId];
				result.Backward = settled_vertices.Backward[best.VertexId];
			}
			return result;
		}

		/// <summary>
		/// Test stopping conditions and output the best tentative route.
		/// </summary>
		/// <param name="settled_forward_vertices"></param>
		/// <param name="settled_backward_vertices"></param>
		/// <returns></returns>
		private CHBest CalculateBest(CHQueue queue)
		{
			CHBest best = new CHBest();
			best.VertexId = -1;
			best.Weight = double.MaxValue;

			// loop over all intersections.
			foreach (KeyValuePair<long, float> vertex in queue.Intersection)
			{
				//if (vertex != best.VertexId)
				//{ // don't retry the same vertex.
				double weight = vertex.Value;
				if (weight < best.Weight)
				{
					best = new CHBest();
					best.VertexId = vertex.Key;
					best.Weight = weight;
				}
				//}
			}
			return best;
		}

		/// <summary>
		/// Holds the result.
		/// </summary>
		private struct CHBest
		{
			/// <summary>
			/// The vertex in the 'middle' of the best route yet.
			/// </summary>
			public long VertexId { get; set; }

			/// <summary>
			/// The weight of the best route yet.
			/// </summary>
			public double Weight { get; set; }
		}


		private struct CHResult
		{
			public CHPathSegment Forward { get; set; }

			public CHPathSegment Backward { get; set; }
		}

		/// <summary>
		/// Do one forward search step.
		/// </summary>
		/// <param name="queue"></param>
		/// <returns></returns>
		private void SearchForward(CHQueue settled_queue, CHPriorityQueue queue,
		                           long exception)
		{
			// get the current vertex with the smallest weight.
			CHPathSegment current = queue.Pop();

			// add to the settled vertices.
			CHPathSegment previous_linked_route;
			if (settled_queue.Forward.TryGetValue(current.VertexId, out previous_linked_route))
			{
				if (previous_linked_route.Weight > current.Weight)
				{
					// settle the vertex again if it has a better weight.
					settled_queue.AddForward(current);
					//settled_vertices[current.VertexId] = current;
				}
			}
			else
			{
				// settled the vertex.
				settled_queue.AddForward(current);
				//settled_vertices[current.VertexId] = current;
			}

			// get neighbours.
			CHVertex vertex = this.GetCHVertex(current.VertexId);

			// add the neighbours to the queue.
			foreach (CHVertexNeighbour neighbour in vertex.ForwardNeighbours)
			{
				if (!settled_queue.Forward.ContainsKey(neighbour.Id)
				    && exception != neighbour.Id)
				{
					// if not yet settled.
					CHPathSegment route_to_neighbour = new CHPathSegment(
						neighbour.Id, current.Weight + neighbour.Weight, current);
					queue.Push(route_to_neighbour);
				}
			}
		}

		/// <summary>
		/// Do one backward search step.
		/// </summary>
		/// <param name="queue"></param>
		/// <returns></returns>
		private void SearchBackward(CHQueue settled_queue, CHPriorityQueue queue,
		                            long exception)
		{
			// get the current vertex with the smallest weight.
			CHPathSegment current = queue.Pop();

			// add to the settled vertices.
			CHPathSegment previous_linked_route;
			if (settled_queue.Backward.TryGetValue(current.VertexId, out previous_linked_route))
			{
				if (previous_linked_route.Weight > current.Weight)
				{
					// settle the vertex again if it has a better weight.
					settled_queue.AddBackward(current);
					//settled_vertices[current.VertexId] = current;
				}
			}
			else
			{
				// settled the vertex.
				settled_queue.AddBackward(current);
				//settled_vertices[current.VertexId] = current;
			}

			// get neighbours.
			CHVertex vertex = this.GetCHVertex(current.VertexId);

			// add the neighbours to the queue.
			foreach (CHVertexNeighbour neighbour in vertex.BackwardNeighbours)
			{
				if (!settled_queue.Backward.ContainsKey(neighbour.Id)
				    && exception != neighbour.Id)
				{
					// if not yet settled.
					CHPathSegment route_to_neighbour = new CHPathSegment(
						neighbour.Id, current.Weight + neighbour.Weight, current);
					queue.Push(route_to_neighbour);
				}
			}
		}

		#endregion

		#region Path Expansion

		/// <summary>
		/// Converts the CH paths to complete paths in the orginal network.
		/// </summary>
		/// <param name="forward"></param>
		/// <param name="backward"></param>
		/// <returns></returns>
		private CHPathSegment ExpandPath(CHPathSegment path)
		{
			// construct the full CH path.
			CHPathSegment current = path;
			CHPathSegment expanded_path = null;

			while (current != null && current.From != null)
			{
				// recursively convert edge.
				CHPathSegment local_path =
					new CHPathSegment(current.VertexId, -1, new CHPathSegment(
					                                        	current.From.VertexId));
				CHPathSegment expanded_arc = this.ConvertArc(local_path);
				if (expanded_path != null)
				{
					expanded_path.ConcatenateAfter(expanded_arc);
				}
				else
				{
					expanded_path = expanded_arc;
				}

				current = current.From;
			}
			return expanded_path;
		}

		/// <summary>
		/// Converts the given edge and expands it if needed.
		/// </summary>
		/// <param name="edge"></param>
		/// <returns></returns>
		private CHPathSegment ConvertArc(CHPathSegment edge)
		{
			// get the edge by querying the forward neighbours of the from-vertex.
			CHVertex from_vertex = _data.GetCHVertex(edge.From.VertexId);

			// find the edge with lowest weight.
			CHVertexNeighbour arc = null;
			foreach (CHVertexNeighbour forward_arc in from_vertex.ForwardNeighbours.Where<CHVertexNeighbour>(
				a => a.Id == edge.VertexId))
			{
				if (arc == null)
				{
					arc = forward_arc;
				}
				else if (arc.Weight > forward_arc.Weight)
				{
					arc = forward_arc;
				}
			}
			if (arc == null)
			{
				CHVertex to_vertex = _data.GetCHVertex(edge.VertexId);
				foreach (CHVertexNeighbour backward in to_vertex.BackwardNeighbours.Where<CHVertexNeighbour>(
					a => a.Id == edge.From.VertexId))
				{
					if (arc == null)
					{
						arc = backward;
					}
					else if (arc.Weight > backward.Weight)
					{
						arc = backward;
					}
				}
				return this.ConvertArc(edge, arc.Id, arc.ContractedVertexId, to_vertex.Id);
			}
			else
			{
				return this.ConvertArc(edge, from_vertex.Id, arc.ContractedVertexId, arc.Id);
			}
		}

		private CHPathSegment ConvertArc(CHPathSegment edge,
		                                 long vertex_from_id, long vertex_contracted_id, long vertex_to_id)
		{
			// check if the arc is a shortcut.
			if (vertex_contracted_id > 0)
			{
				// arc is a shortcut.
				CHPathSegment first_path = new CHPathSegment(vertex_to_id, -1,
				                                             new CHPathSegment(vertex_contracted_id));
				CHPathSegment first_path_expanded = this.ConvertArc(first_path);

				CHPathSegment second_path = new CHPathSegment(vertex_contracted_id, -1,
				                                              new CHPathSegment(vertex_from_id));
				CHPathSegment second_path_expanded = this.ConvertArc(second_path);


				// link the two paths.
				first_path_expanded.ConcatenateAfter(second_path_expanded);

				return first_path_expanded;
			}
			return edge;
		}

		#endregion

		#region ICHData

		/// <summary>
		/// Holds all sparse vertices.
		/// </summary>
		private Dictionary<long, CHVertex> _ch_vertices;

		#region Resolve Points

		/// <summary>
		/// Holds a counter 
		/// </summary>
		private int _next_id = -1;

		public CHVertex ResolveAt(long vertex_id)
		{
			return this.GetCHVertex(vertex_id);
		}

		/// <summary>
		/// Returns a resolved point.
		/// </summary>
		/// <param name="coordinate"></param>
		/// <returns></returns>
		public CHVertex Resolve(GeoCoordinate coordinate)
		{
			return this.Resolve(coordinate, 0.001, null);
		}

		/// <summary>
		/// Resolves a point and returns the closest vertex.
		/// </summary>
		/// <param name="coordinate"></param>
		/// <returns></returns>
		private CHVertex Resolve(GeoCoordinate coordinate, double search_radius, ICHVertexMatcher matcher)
		{
			// get the delta.
			double delta = search_radius;

			// initialize the result and distance.
			PotentialResolvedHit best_hit = new PotentialResolvedHit();
			best_hit.Distance = double.MaxValue;

			// keep searching until at least one closeby hit is found.
			while (best_hit.Distance == double.MaxValue && ((matcher != null && delta <= search_radius*2)
			                                                || (matcher == null && delta < search_radius*200)))
			{
				// construct a bounding box.
				GeoCoordinate from_top = new GeoCoordinate(
					coordinate.Latitude + delta,
					coordinate.Longitude - delta);
				GeoCoordinate from_bottom = new GeoCoordinate(
					coordinate.Latitude - delta,
					coordinate.Longitude + delta);

				// query datasource.
				IEnumerable<CHVertex> query_result =
					this.GetCHVertices(new GeoCoordinateBox(from_top, from_bottom));
				foreach (CHVertex vertex in query_result)
				{
					// get the neighbours
					foreach (CHVertexNeighbour neighbour in vertex.ForwardNeighbours)
					{
						if (neighbour.ContractedVertexId < 0)
						{
							// only do the contracted versions.
							// resolve between the two neighbours.
							PotentialResolvedHit potential_hit = this.ResolveBetween(coordinate,
							                                                         vertex.Id, neighbour.Id,
							                                                         vertex, this.GetCHVertex(neighbour.Id));

							// keep the result only if it is better.
							if (potential_hit.Distance < best_hit.Distance)
							{
								best_hit = potential_hit;
							}
						}
					}
					foreach (CHVertexNeighbour neighbour in vertex.BackwardNeighbours)
					{
						if (neighbour.ContractedVertexId < 0)
						{
							// only do the contracted versions.
							// resolve between the two neighbours.
							PotentialResolvedHit potential_hit = this.ResolveBetween(coordinate,
							                                                         neighbour.Id, vertex.Id,
							                                                         vertex, this.GetCHVertex(vertex.Id));

							// keep the result only if it is better.
							if (potential_hit.Distance < best_hit.Distance)
							{
								best_hit = potential_hit;
							}
						}
					}

					// resolve the vertex itself.
					double result_distance = vertex.Location.DistanceEstimate(coordinate).Value;
					if (result_distance <= best_hit.Distance)
					{
						best_hit = new PotentialResolvedHit();
						best_hit.Distance = result_distance;
						best_hit.Vertex = vertex;
					}
				}

				// get the resolved versions.

				delta = delta*2;
			}

			// process the best result.
			CHVertex result = null;
			if (best_hit.Vertex != null)
			{
				// the best result was a sparse vertex.
				result = best_hit.Vertex;
			}
			else
			{
				// the best result was somewhere in between vertices.
				LineProjectionResult<GeoCoordinate> line_projection = best_hit.LineProjection;

				// get the actual intersection point.
				GeoCoordinate intersection_point = line_projection.ClosestPrimitive as GeoCoordinate;

				// process the projection.
				double latitude_diff = best_hit.Neighbour2.Latitude - best_hit.Neighbour1.Latitude;
				double latitude_diff_small = intersection_point.Latitude - best_hit.Neighbour1.Latitude;
				float position = (float) System.Math.Max(System.Math.Min((latitude_diff_small/latitude_diff), 1), 0);
				if (latitude_diff == 0 && latitude_diff_small == 0)
				{
					position = 0;
				}

				// create the result.
				if (position == 0)
				{
					// the position is at the first node @ line_projection.Idx.
					if (line_projection.Idx == 0)
					{
						// the closest one is the sparse vertex.
						result = best_hit.Neighbour1;
					}
					else if (line_projection.Idx == 1)
					{
						// the closest one is another vertex.
						result = best_hit.Neighbour2;
					}
				}
				else if (position == 1)
				{
					// the position is at the first node @ line_projection.Idx + 1.
					if (line_projection.Idx == 1)
					{
						// the closest node is neighbour2
						result = best_hit.Neighbour2;
					}
					else
					{
						// the closest one is another vertex.
						result = best_hit.Neighbour1;
					}
				}
				else
				{
					// the best location is not located at an existing vertex.
					// the position is somewhere in between.
					result = best_hit.Neighbour1;

					//result = new CHVertex();
					//result.Id = _next_id;
					//_next_id--;
					//result.Level = -1;
					//result.Latitude = intersection_point.Latitude;
					//result.Longitude = intersection_point.Longitude;
					//_ch_vertices[result.Id] = result;

					//// get the forward arcs.
					//CHArc neighbour_forward = null;
					//HashSet<CHArc> neighbours_forward = this.GetCHArcs(best_hit.Neighbour1.Id, -1);
					//foreach (CHArc potential_neighbour in neighbours_forward)
					//{ // loop over all forward neighbours.
					//    if (potential_neighbour.VertexToId == best_hit.Neighbour2.Id)
					//    { // do not add the unmodified neighbours.
					//        neighbour_forward = potential_neighbour;
					//    }
					//    else
					//    { // add the unmodified neighbours.
					//        this.ResolvedArc(potential_neighbour);
					//    }
					//}                    

					//// adjust the neighbour.
					//double forward_weight = neighbour_forward.Weight * position;

					//// add a forward neighbour to the new vertex.
					//CHArc forward1 = new CHArc();
					//forward1.Weight = forward_weight;
					//forward1.VertexFromId = neighbour_forward.VertexFromId;
					//forward1.VertexToId = result.Id;
					//forward1.Tags = neighbour_forward.Tags;
					//this.ResolvedArc(forward1);

					//// add a forward neighbour to the new vertex.
					//CHArc forward2 = new CHArc();
					//forward2.Weight = neighbour_forward.Weight - forward_weight;
					//forward2.VertexFromId = result.Id;
					//forward2.VertexToId = neighbour_forward.VertexToId;
					//forward2.Tags = neighbour_forward.Tags;
					//this.ResolvedArc(forward2);

					//CHArc neighbour_backward = null;
					//HashSet<CHArc> neighbours_backward = this.GetCHArcsReversed(best_hit.Neighbour1.Id, -1);
					//foreach (CHArc potential_neighbour in neighbours_backward)
					//{ // loop over all backward neighbours.
					//    if (potential_neighbour.VertexFromId == best_hit.Neighbour2.Id)
					//    {
					//        neighbour_backward = potential_neighbour;
					//    }
					//    else
					//    { // add the unmodified neighbours.
					//        this.ResolvedArc(potential_neighbour);
					//    }
					//}

					//// adjust the neighbour.
					//double backward_weight = neighbour_backward.Weight * (1.0 - position);

					//// add a forward neighbour to the new vertex.
					//CHArc backward1 = new CHArc();
					//backward1.Weight = forward_weight;
					//backward1.VertexFromId = neighbour_forward.VertexFromId;
					//backward1.VertexToId = result.Id;
					//backward1.Tags = neighbour_forward.Tags;
					//this.ResolvedArc(backward1);

					//// add a forward neighbour to the new vertex.
					//CHArc backward2 = new CHArc();
					//backward2.Weight = neighbour_forward.Weight - forward_weight;
					//backward2.VertexFromId = result.Id;
					//backward2.VertexToId = neighbour_forward.VertexToId;
					//backward2.Tags = neighbour_forward.Tags;
					//this.ResolvedArc(backward2);

					//// get the forward arcs.
					//neighbours_forward = this.GetCHArcs(best_hit.Neighbour2.Id, -1);
					//foreach (CHArc potential_neighbour in neighbours_forward)
					//{ // loop over all forward neighbours.
					//    if (potential_neighbour.VertexToId == best_hit.Neighbour1.Id)
					//    { // do not add the unmodified neighbours.
					//        neighbour_forward = potential_neighbour;
					//    }
					//    else
					//    { // add the unmodified neighbours.
					//        this.ResolvedArc(potential_neighbour);
					//    }
					//}

					//// adjust the neighbour.
					//forward_weight = neighbour_forward.Weight * position;

					//// add a forward neighbour to the new vertex.
					//forward1 = new CHArc();
					//forward1.Weight = forward_weight;
					//forward1.VertexFromId = neighbour_forward.VertexFromId;
					//forward1.VertexToId = result.Id;
					//forward1.Tags = neighbour_forward.Tags;
					//this.ResolvedArc(forward1);

					//// add a forward neighbour to the new vertex.
					//forward2 = new CHArc();
					//forward2.Weight = neighbour_forward.Weight - forward_weight;
					//forward2.VertexFromId = result.Id;
					//forward2.VertexToId = neighbour_forward.VertexToId;
					//forward2.Tags = neighbour_forward.Tags;
					//this.ResolvedArc(forward2);

					//neighbours_backward = this.GetCHArcsReversed(best_hit.Neighbour2.Id, -1);
					//foreach (CHArc potential_neighbour in neighbours_backward)
					//{ // loop over all backward neighbours.
					//    if (potential_neighbour.VertexFromId == best_hit.Neighbour1.Id)
					//    {
					//        neighbour_backward = potential_neighbour;
					//    }
					//    else
					//    { // add the unmodified neighbours.
					//        this.ResolvedArc(potential_neighbour);
					//    }
					//}

					//// adjust the neighbour.
					//backward_weight = neighbour_backward.Weight * (1.0 - position);

					//// add a forward neighbour to the new vertex.
					//backward1 = new CHArc();
					//backward1.Weight = forward_weight;
					//backward1.VertexFromId = neighbour_forward.VertexFromId;
					//backward1.VertexToId = result.Id;
					//backward1.Tags = neighbour_forward.Tags;
					//this.ResolvedArc(backward1);

					//// add a forward neighbour to the new vertex.
					//backward2 = new CHArc();
					//backward2.Weight = neighbour_forward.Weight - forward_weight;
					//backward2.VertexFromId = result.Id;
					//backward2.VertexToId = neighbour_forward.VertexToId;
					//backward2.Tags = neighbour_forward.Tags;
					//this.ResolvedArc(backward2);
				}
			}
			return result;
		}

		/// <summary>
		/// Resolves the closest point between the given neighbours.
		/// </summary>
		/// <param name="neighbour1_id"></param>
		/// <param name="neighbour2_id"></param>
		/// <param name="neighbour1"></param>
		/// <param name="neighbour2"></param>
		/// <returns></returns>
		private PotentialResolvedHit ResolveBetween(GeoCoordinate coordinate, long neighbour1_id, long neighbour2_id,
		                                            CHVertex neighbour1, CHVertex neighbour2)
		{
			// intersect with the neighbours line.
			List<GeoCoordinate> points = new List<GeoCoordinate>();
			points.Add(neighbour1.Location);
			points.Add(neighbour2.Location);

			// define the line and intersect.
			ShapePolyLineF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine> line =
				new ShapePolyLineF<GeoCoordinate, GeoCoordinateBox, GeoCoordinateLine>(PrimitiveGeoFactory.Instance,
				                                                                       points.ToArray());
			LineProjectionResult<GeoCoordinate> line_projection =
				(line.DistanceDetailed(coordinate) as LineProjectionResult<GeoCoordinate>);

			PotentialResolvedHit best_hit = new PotentialResolvedHit();
			best_hit.Distance = line_projection.Distance;
			best_hit.LineProjection = line_projection;
			best_hit.Neighbour1 = neighbour1;
			best_hit.Neighbour2 = neighbour2;
			return best_hit;
		}

		/// <summary>
		/// Potential resolved hit.
		/// </summary>
		private struct PotentialResolvedHit
		{
			public double Distance { get; set; }

			public CHVertex Vertex { get; set; }

			#region Non-Direct hits

			public LineProjectionResult<GeoCoordinate> LineProjection { get; set; }

			public CHVertex Neighbour1 { get; set; }

			public CHVertex Neighbour2 { get; set; }

			#endregion
		}

		#endregion

		#region ICHData

		/// <summary>
		/// Returns the vertex with the given id.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public CHVertex GetCHVertex(long id)
		{
			CHVertex vertex = null;
			if (_ch_vertices.TryGetValue(id, out vertex))
			{
				return vertex;
			}
			return _data.GetCHVertex(id);
		}

		public IEnumerable<CHVertex> GetCHVerticesNoLevel()
		{
			return _data.GetCHVerticesNoLevel();
		}

		/// <summary>
		/// Persists the given vertex.
		/// </summary>
		/// <param name="vertex"></param>
		public void PersistCHVertex(CHVertex vertex)
		{
			_data.PersistCHVertex(vertex);
		}

		public void PersistCHVertexNeighbour(CHVertex vertex, CHVertexNeighbour arc, bool forward)
		{
			_data.PersistCHVertexNeighbour(vertex, arc, forward);
		}

		/// <summary>
		/// Deletes the vertex with the given id.
		/// </summary>
		/// <param name="id"></param>
		public void DeleteCHVertex(long id)
		{
			_data.DeleteCHVertex(id);
		}

		public void DeleteNeighbours(long vertexid)
		{
			_data.DeleteNeighbours(vertexid);
		}

		public void DeleteNeighbour(CHVertex vertex, CHVertexNeighbour neighbour, bool forward)
		{
			_data.DeleteNeighbour(vertex, neighbour, forward);
		}

		/// <summary>
		/// Returns the vertices inside the given box.
		/// </summary>
		/// <param name="box"></param>
		/// <returns></returns>
		public IEnumerable<CHVertex> GetCHVertices(GeoCoordinateBox box)
		{
			HashSet<CHVertex> vertices_in_box = new HashSet<CHVertex>();
			foreach (CHVertex vertex in _ch_vertices.Values)
			{
				if (box.IsInside(vertex.Location))
				{
					vertices_in_box.Add(vertex);
				}
			}
			IEnumerable<CHVertex> vertices = _data.GetCHVertices(box);
			if (vertices != null)
			{
				foreach (CHVertex vertex in vertices)
				{
					vertices_in_box.Add(vertex);
				}
			}
			return vertices_in_box;
		}

		#endregion

		#endregion

		#region Notifications

		/// <summary>
		/// The delegate for arc notifications.
		/// </summary>
		/// <param name="arc"></param>
		/// <param name="contracted_id"></param>
		public delegate void NotifyCHPathSegmentDelegate(CHPathSegment route);

		/// <summary>
		/// The event.
		/// </summary>
		public event NotifyCHPathSegmentDelegate NotifyCHPathSegmentEvent;

		/// <summary>
		/// Notifies the arc.
		/// </summary>
		/// <param name="arc"></param>
		/// <param name="contracted_id"></param>
		private void NotifyCHPathSegment(CHPathSegment route)
		{
			if (this.NotifyCHPathSegmentEvent != null)
			{
				this.NotifyCHPathSegmentEvent(route);
			}
		}

		#endregion

	}

}
