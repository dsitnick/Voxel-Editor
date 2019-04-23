using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxCanvasRenderer : MonoBehaviour {

    private struct VoxelDelta {
        public Color? color; //Eventually change to Voxel data

        public VoxelDelta (Color? color) {
            this.color = color;
        }
    }
    public ParticleSystem system;

    private ParticleSystem.Particle defaultParticle;
    private ParticleSystem.Particle deadParticle;

    private ParticleSystem.Particle[] particleBuffer = new ParticleSystem.Particle[65535 / 12];

    private Dictionary<uint, Vector3Int> idMap;
    private Dictionary<Vector3Int, VoxelDelta> pendingDeltas;

    private uint nextId;

    void Awake () {
        defaultParticle = new ParticleSystem.Particle {
            remainingLifetime = float.MaxValue,
            startSize = 1
        };

        deadParticle = new ParticleSystem.Particle {
            remainingLifetime = -1,
            startSize = 0
        };

        idMap = new Dictionary<uint, Vector3Int> ();
        pendingDeltas = new Dictionary<Vector3Int, VoxelDelta> ();
        nextId = 0;
    }

    void LateUpdate () {

        int count = system.GetParticles (particleBuffer);

        if (pendingDeltas.Count > 0) {
            for (int i = 0; i < count; i++) {
                uint id = particleBuffer[i].randomSeed;

                if (idMap.ContainsKey (id)) {
                    Vector3Int pos = idMap[id];

                    if (pendingDeltas.ContainsKey (pos)) {
                        VoxelDelta delta = pendingDeltas[pos];

                        if (delta.color.HasValue) {
                            SetParticle (i, pos, delta, id);
                        } else {
                            RemoveParticle (i);
                            idMap.Remove (id);
                        }

                        pendingDeltas.Remove (pos);
                    }
                }
            }
        }

        int addIndex = count;

        foreach (KeyValuePair<Vector3Int, VoxelDelta> p in pendingDeltas) {
            if (p.Value.color.HasValue) {
                SetParticle (addIndex, p.Key, p.Value, nextId);
                idMap[nextId] = p.Key;

                nextId++;
                addIndex++;
            }
        }

        pendingDeltas.Clear ();

        system.SetParticles (particleBuffer, addIndex);
    }

    private void SetParticle (int index, Vector3Int pos, VoxelDelta value, uint id) {
        defaultParticle.position = pos;
        defaultParticle.startColor = value.color.Value;
        defaultParticle.randomSeed = id;
        particleBuffer[index] = defaultParticle;
    }

    private void RemoveParticle (int index) {
        particleBuffer[index] = deadParticle;
    }

    public void Place (Vector3Int pos, Color color) {
        pendingDeltas[pos] = new VoxelDelta (color);
    }

    public void Erase (Vector3Int pos) {
        pendingDeltas[pos] = new VoxelDelta (null);
    }

}
