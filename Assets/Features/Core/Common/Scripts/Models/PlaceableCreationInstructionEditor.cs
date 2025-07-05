using UnityEngine;

namespace Features.Core.Common.Models
{
    public class PlaceableCreationInstructionEditor : MonoBehaviour
    {
        [SerializeField] private PlaceableCreationInstruction _instruction;

        public PlaceableCreationInstruction GetInstruction()
        {
            return _instruction;
        }
    }
}