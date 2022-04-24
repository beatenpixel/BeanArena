using System;
using System.Collections;
using System.Collections.Generic;

namespace ExitGames.Client.Photon {
    // Token: 0x02000006 RID: 6
    public class DictionaryEntryEnumerator : IEnumerator<DictionaryEntry>, IEnumerator, IDisposable {
        // Token: 0x0600003C RID: 60 RVA: 0x00002FFC File Offset: 0x000011FC
        public DictionaryEntryEnumerator(IDictionaryEnumerator original) {
            this.enumerator = original;
        }

        // Token: 0x0600003D RID: 61 RVA: 0x00003010 File Offset: 0x00001210
        public bool MoveNext() {
            return this.enumerator.MoveNext();
        }

        // Token: 0x0600003E RID: 62 RVA: 0x0000302D File Offset: 0x0000122D
        public void Reset() {
            this.enumerator.Reset();
        }

        // Token: 0x1700000D RID: 13
        // (get) Token: 0x0600003F RID: 63 RVA: 0x0000303C File Offset: 0x0000123C
        object IEnumerator.Current {
            get {
                return (DictionaryEntry)this.enumerator.Current;
            }
        }

        // Token: 0x1700000E RID: 14
        // (get) Token: 0x06000040 RID: 64 RVA: 0x00003064 File Offset: 0x00001264
        public DictionaryEntry Current {
            get {
                return (DictionaryEntry)this.enumerator.Current;
            }
        }

        // Token: 0x1700000F RID: 15
        // (get) Token: 0x06000041 RID: 65 RVA: 0x00003088 File Offset: 0x00001288
        public object Key {
            get {
                return this.enumerator.Key;
            }
        }

        // Token: 0x17000010 RID: 16
        // (get) Token: 0x06000042 RID: 66 RVA: 0x000030A8 File Offset: 0x000012A8
        public object Value {
            get {
                return this.enumerator.Value;
            }
        }

        // Token: 0x17000011 RID: 17
        // (get) Token: 0x06000043 RID: 67 RVA: 0x000030C8 File Offset: 0x000012C8
        public DictionaryEntry Entry {
            get {
                return this.enumerator.Entry;
            }
        }

        // Token: 0x06000044 RID: 68 RVA: 0x000030E5 File Offset: 0x000012E5
        public void Dispose() {
            this.enumerator = null;
        }

        // Token: 0x04000015 RID: 21
        private IDictionaryEnumerator enumerator;
    }
}
