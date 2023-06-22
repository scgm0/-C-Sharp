using System;
using System.ComponentModel;
using Godot;

namespace 球武道.scripts;

public partial class Ball : RigidBody2D {
	private 设定.境界 _境界;
	private int _年龄;
	private int _生命 = 100;
	private int _生命上限 = 100;
	private int _寿命 = 50;
	private double _修为;
	private int _tipIndex0;
	private int _tipIndex1;
	public int 累计修为;

	public string 名字;
	public string 身份;
	public int 修为上限 = 120;
	public bool 已死 = false;
	public double 资质 = 0.2;
	public MeshInstance2D Body;

	[Export]
	public 设定.境界 境界 {
		set {
			if (!Enum.IsDefined(typeof(设定.境界), value)) {
				throw new InvalidEnumArgumentException(nameof(value));
			}

			_境界 = value;
			Mass = (int)境界 + 1;
			GetNode<Label>("%境界").Text = 境界.ToString();
			GetNode<Label>("%境界").AddThemeFontSizeOverride("font_size", 16 + (int)境界 * 2);
			GetNode<Label>("%境界").AddThemeColorOverride("font_outline_color",
				new Color((float)((double)境界 / 10.0), (float)((double)境界 / 10.0), (float)((double)境界 / 10.0)));
		}
		get => _境界;
	}

	public double 修为 {
		private set {
			_修为 = Mathf.Snapped(value, 0.1);
			((ShaderMaterial)GetNode<MeshInstance2D>("%修为").Material).SetShaderParameter("fill_ratio", 修为 / 修为上限 * 0.5);
			if (修为 > 修为上限) {
				突破();
			}
		}
		get => _修为;
	}

	public int 寿命 {
		set {
			_寿命 = value;
			GetNode<Label>("%年龄").Text = GlData.GetAgeGroup(年龄, 寿命);
		}
		get => _寿命;
	}

	public int 年龄 {
		set {
			_年龄 = value;
			GetNode<Label>("%年龄").Text = GlData.GetAgeGroup(年龄, 寿命);
		}
		get => _年龄;
	}

	public int 生命上限 {
		set {
			_生命上限 = value;
			((ShaderMaterial)GetNode<MeshInstance2D>("%生命").Material).SetShaderParameter("fill_ratio", (double)生命 / 生命上限 * 0.5);
		}
		get => _生命上限;
	}

	public int 生命 {
		set {
			_生命 = Mathf.Max(value, 生命上限);
			((ShaderMaterial)GetNode<MeshInstance2D>("%生命").Material).SetShaderParameter("fill_ratio", (double)生命 / 生命上限 * 0.5);
		}
		get => _生命;
	}

	public override void _Ready() {
		Body = GetNode<MeshInstance2D>("Body");
		Body.Modulate = 设定.阵营[身份];
		GetNode<Label>("%名字").Text = 名字;
	}

	public void 过月() {
		修为 += 资质;
	}

	public void 突破() {
		if (!已死) {
			var self = this;
			累计修为 += 修为上限;
			修为 -= 修为上限;
			境界 += 1;
			var 属性 = 设定.属性[境界];
			self *= 属性;
		}
	}

	public static Ball operator +(Ball a, 属性值 b) {
		a.寿命 += Mathf.CeilToInt(b.寿命 ?? 0.0);
		a.年龄 += Mathf.CeilToInt(b.年龄 ?? 0.0);
		a.生命上限 += Mathf.CeilToInt(b.生命上限 ?? 0.0);
		a.生命 += Mathf.CeilToInt(b.生命 ?? 0.0);
		a.修为上限 += Mathf.CeilToInt(b.修为上限 ?? 0);
		a.修为 += b.修为 ?? 0;
		a.资质 += b.资质 ?? 0;
		return a;
	}

	public static Ball operator *(Ball a, 属性值 b) {
		a.寿命 = (int)(a.寿命 * b.寿命 ?? 1);
		a.年龄 = (int)(a.年龄 * b.年龄 ?? 1);
		a.生命上限 = (int)(a.生命上限 * b.生命上限 ?? 1);
		a.生命 = (int)(a.生命 * b.生命 ?? 1);
		a.修为上限 = (int)(a.修为上限 * b.修为上限 ?? 1);
		a.修为 *= b.修为 ?? 1;
		a.资质 *= b.资质 ?? 1;
		return a;
	}
}